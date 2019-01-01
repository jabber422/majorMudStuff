using MMudObjects;
using MMudTerm_Protocols;
using MMudTerm_Protocols.AnsiProtocolCmds;
using MMudTerm_Protocols.Script;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Linq;

namespace HomeList
{
    internal class Script : ScriptBase
    {
        public Dictionary<string, TrackedPlayer> Players = new Dictionary<string, TrackedPlayer>();
        Regex whoLine = new Regex(@"(Good|Lawful|Saint|Seedy|Villain|Outlaw|Fiend)? (\w+) (\w+) *-  (\w+)(  of(.*))?");
        Regex topLine = new Regex(@"\s\s?(\d+)\. (\w+)(\s\w+)?\s+(\w+)\s+(\w+ )+(\s+)?(\d+)");

        public Script(ConnObj connObj) : base(connObj)
        {
            this.decoder = new AnsiProtocolDecoder(connObj);
        }

        public override void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.m_workerThread.CancellationPending)
            {
                //block until conObj gets something
                this.decoder.mre.WaitOne();
                this.decoder.mre.Reset();
                List<ProtocolCommandLine> Cmds = this.decoder.GetCommandLines();
                //possible thread issue, we lock on cmds, so conObj reciever can not add anything while we process.  we could back up the input stream and blow up?
                lock (Cmds)
                {
                    if (Cmds.Count == 0) continue;

                    Boolean IsNewData = false;
                    foreach(ProtocolCommandLine c in Cmds){ 
                        String text = c.ToString();

                        if (text.Contains("ID?"))
                        {
                            Send("ID,2,42");
                        }

                        if (whoLine.IsMatch(text))
                        {
                            HandleNewWhoLineData(text);
                            IsNewData = true;
                        }
                        else if (topLine.IsMatch(text))
                        {
                            HandleNewTopLineData(text);
                            IsNewData = true;
                        }
                    }

                    if (IsNewData)
                    {
                        Dictionary<string, TrackedPlayer> PlayerCopy = new Dictionary<string, TrackedPlayer>(this.Players);
                        this.m_workerThread.ReportProgress(1, PlayerCopy);
                    }
                }
            }
        }

        private void HandleNewWhoLineData(string text)
        {
            MatchCollection mc = whoLine.Matches(text);
            if (mc.Count > 1)
            {
                Trace.WriteLine("Script - Regex match has more that 1 match, should be 1", "HomeList.Script");
                throw new NotImplementedException("fix this, will it even happen?");
            }

            Match m = mc[0];
            if (!m.Success)
            {
                Trace.WriteLine("Script - have match but not success?? wtf does that even mean?");
                throw new NotImplementedException("--fix this, will it even happen?");
            }

            Player playerInfo = new Player(m.Groups[2].Value)
            {
                Alignment = m.Groups[1].Value,
                Title = m.Groups[4].Value,
                GangName = m.Groups[6].Value,
            };

            TrackedPlayer trackedPlayerInfo = GetPlayerInfo(playerInfo.FirstName);
            trackedPlayerInfo.UpdateTrackedPlayer(playerInfo);
        }

        private void HandleNewTopLineData(string text)
        {
            MatchCollection mc = topLine.Matches(text);
            if (mc.Count > 1)
            {
                string msg = "Script.HandleNewTopLineData - Regex match has more that 1 match, should be 1";
                Trace.WriteLine(msg);
                throw new NotImplementedException(msg);
            }

            Match m = mc[0];
            if (!m.Success)
            {
                string msg = "Script.HandleNewTopLineData - have match but not success?? wtf does that even mean?";
                Trace.WriteLine(msg);
                throw new NotImplementedException(msg);
            }
         
            Player playerInfo = new Player(m.Groups[2].Value)
            {
                Rank = int.Parse(m.Groups[1].Value),
                GangName = m.Groups[5].Value,
                Exp = double.Parse(m.Groups[7].Value),
                //LastName = m.Groups[3].Value
            };

            playerInfo.Stats.Class = PlayableClassFactory.CreteClassFromName(m.Groups[4].Value);

            if (playerInfo.Exp < 9999)
            {

            }
            TrackedPlayer trackedPlayerInfo = GetPlayerInfo(playerInfo.FirstName);
            trackedPlayerInfo.UpdateTrackedPlayer(playerInfo);
        }

        private TrackedPlayer GetPlayerInfo(string firstName)
        {
            TrackedPlayer player = null;
            if (this.Players.ContainsKey(firstName))
            {
                player = this.Players[firstName];
            }
            else
            {
                player = new TrackedPlayer(firstName);
                this.Players.Add(firstName, player);
            }

            return player;
        }
    }
}
