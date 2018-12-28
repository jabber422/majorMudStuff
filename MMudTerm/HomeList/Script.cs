using MMudObjects;
using MMudTerm_Protocols;
using MMudTerm_Protocols.AnsiProtocolCmds;
using MMudTerm_Protocols.Script;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;

namespace HomeList
{
    internal class Script : ScriptBase
    {
        public event EventHandler NewWhoInfo;
        public Dictionary<string, Player> Players = new Dictionary<string, Player>();

        

        Regex whoLine = new Regex(@"(Good|Lawful|Saint|Seedy|Villain|Outlaw|Fiend)? (\w+) (\w+) *-  (\w+)(  of(.*))?");
        private string fName;

        public Script(ConnObj connObj) : base(connObj)
        {
            this.decoder = new AnsiProtocolDecoder();
        }

        public override void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.m_workerThread.CancellationPending)
            {
                lock (this.Cmds)
                {
                    //block until conObj gets something
                    this.mre.WaitOne();
                    this.mre.Reset();
                    if (this.Cmds.Count == 0) continue;

                    Boolean IsNewData = false;
                    while (this.Cmds.Count > 0)
                    {
                        ProtocolCommand c = Cmds.Dequeue();

                        String text = c.ToString();

                        if (text.Contains("ID?"))
                        {
                            Send("ID,2,42");
                        }

                        MatchCollection mc = whoLine.Matches(text);
                        if (mc.Count == 0) continue;
                        else if (mc.Count > 1)
                        {
                            Trace.WriteLine("Script - Regex match has more that 1 match, should be 1", "HomeList.Script");
                        }

                        Match m = mc[0];
                        if (!m.Success)
                        {
                            Trace.WriteLine("Script - have match but not success?? wtf does that even mean?");
                        }

                        string Alignment = m.Groups[1].Value;
                        string fName = m.Groups[2].Value;
                        string lName = m.Groups[3].Value;
                        string Title = m.Groups[4].Value;
                        string Gang = m.Groups[5].Value;

                        if (!this.Players.ContainsKey(fName))
                        {
                            this.Players.Add(fName, new Player(fName));
                        }
                        this.Players[fName].Alignment = Alignment;
                        this.Players[fName].LastName = lName;
                        this.Players[fName].Title = Title;
                        this.Players[fName].GangName = Gang;

                        IsNewData = true;
                    }

                    if (IsNewData)
                    {
                        Dictionary<string, Player> PlayerCopy = new Dictionary<string, Player>(Players);
                        this.m_workerThread.ReportProgress(1, PlayerCopy);
                    }
                }
            }
        }
    }
}
