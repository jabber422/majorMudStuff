using MMudObjects;
using MMudTerm.Game;
using MMudTerm_Protocols;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace MMudTerm.Session.SessionStateData
{
    internal class SessionStateInGame : SessionState
    {
        //[HP=73 (Resting) ]: 
        //[HP=85/KAI=4]: (Resting)    
        //[HP=185/MP=22]: (Resting)    
        //[HP=73]:
        private string pattern_tick_caster = @"\[HP=(\d+)(?:/(?:KAI|MA)=(\d+))?\]:(?: \((\w+)\))?";
        private string pattern_tick_nomana = @"\[HP=(\d+)(?:]:| \((\S+)\) ]:)";
        private string pattern_to_use;
        
        private Regex _resting = null;
        private StringBuilder buffer = new StringBuilder();

        private MajorMudBbsGame _gameenv = null;

        //public delegate void NewGameEventHandler(EventType message);

        // Define the event based on the delegate
        //public event NewGameEventHandler NewGameEvent;

        public SessionStateInGame(SessionState _state) : base(_state, "In Game")
        {
            this._gameenv = this.m_controller._gameenv;
            this._resting = new Regex(@"\((Resting)\)", RegexOptions.Compiled);
            this._gameenv.NewGameEvent += this.m_controller.m_sessionForm.Update;
            this.m_controller.Send("\r\n");
            //this.m_controller.Send("stat\r\n");
            //this.m_controller.Send("i\r\n");
            //this.m_controller.Send("who\r\n");
        }
        
        //takes a queue of TermCmds and turns it into a string with \r\n preserved...
        internal override SessionState HandleCommands(Queue<TermCmd> cmds)
        {
            string s = "";
            string q = "";
            foreach (TermCmd c in cmds)
            {
                if (c is TermStringDataCmd)
                {
                    s += (c as TermStringDataCmd).GetValue();
                    q += (c as TermStringDataCmd).GetValue();
                }
                else if (c is TermNewLineCmd)
                {
                    s += '\n';
                    q += '\n';
                }
                else if (c is TermCarrigeReturnCmd)
                {
                    s += '\r';
                    q += '\r';
                }else if (c is AnsiGraphicsCmd)
                {
                    q += "#" + string.Join(",", (c as AnsiGraphicsCmd).vals) + "#";
                }
            }
            //Console.WriteLine(q +"\r\n");

            var new_state = Handle(s, q);
            return new_state;
        }

        //processes the text block, deals with partial strings, and chunks eveything use HP=X/MA=x as the delimiter
        public SessionState Handle(string incomingString, string incomingStringWithColorTokens)
        {
            //before we do anything we need to figure out what the [HP= type of the player is

            
            //Console.WriteLine(incomingStringWithColorTokens);
            buffer.Append(incomingString);
            string bufferContent = buffer.ToString();

            if(this.pattern_to_use == null)
            {
                var tokens = bufferContent.Split(new string[] { "[HP=" }, StringSplitOptions.None);
                if (tokens.Length < 2) return this;
                if (tokens[1].Contains("/"))
                {
                    //mana/kai user
                    pattern_to_use = pattern_tick_caster;
                }
                else
                {
                    pattern_to_use = pattern_tick_nomana;
                }
                this.m_controller.Send("stat\r");
                this.m_controller.Send("i\r");
                this.m_controller.Send("who\r");
                buffer.Clear();
                return this;
            }

            MatchCollection matches = Regex.Matches(bufferContent, pattern_to_use);
            int lastProcessedIndex = 0;
            foreach (Match match in matches)
            {
                string toProcess = bufferContent.Substring(lastProcessedIndex, match.Index - lastProcessedIndex);
                if (toProcess == "") continue;
                this._gameenv.Process(toProcess);
                if (this._gameenv.result != EventType.None)
                {
                    this._gameenv.HandleNewGameEvent(this._gameenv.result, this._gameenv.result_data);
                }
                else
                {

                }
                this._gameenv.ProcessTick(match, "");
                this._gameenv.HandleNewGameEvent(this._gameenv.result, this._gameenv.result_data);
                lastProcessedIndex = match.Index + match.Length;
            }

            buffer.Clear();
            buffer.Append(bufferContent.Substring(lastProcessedIndex));
            lastProcessedIndex = 0;
            return this;

            ////var tokens = Regex.Split(bufferContent, )













            //if (bufferContent.StartsWith(" (Resting) "))
            //{
            //    this._gameenv._player.IsResting = true;
            //    bufferContent = bufferContent.Substring(" (Resting) ".Length);
            //}

            ////MatchCollection matches = Regex.Matches(bufferContent, pattern_tick);

            //Dictionary<string, List<(EventType, Object)>> common_patterns = new Dictionary<string, List<(EventType, object)>>();
            //common_patterns.Add(pattern_tick, new List<(EventType, object)> { (EventType.Tick, null) });
            //common_patterns.Add(pattern_tick2, new List<(EventType, object)> { (EventType.Tick, null) });
            //RegexMatcher rm = new RegexMatcher(common_patterns);
            //var res = rm.TryMatch(bufferContent, out Match match, out List<(EventType, object)> callback);

            //int lastProcessedIndex = 0;
            //foreach (Match match in matches)
            //{
            //    string toProcess = bufferContent.Substring(lastProcessedIndex, match.Index - lastProcessedIndex);
            //    this._gameenv.Process(toProcess);
            //    if (this._gameenv.result != EventType.None)
            //    {
            //        //Console.WriteLine("-------------------------------");
            //        //Console.WriteLine(this._gameenv.result);
            //        //Console.WriteLine(incomingString);
            //        //Console.WriteLine("-------------------------------");

            //        this._gameenv.HandleNewGameEvent(this._gameenv.result, this._gameenv.result_data);
            //    }
            //    this._gameenv.ProcessTick(match, "");
            //    this._gameenv.HandleNewGameEvent(this._gameenv.result, this._gameenv.result_data);
            //    lastProcessedIndex = match.Index + match.Length;
            //}

            //buffer.Clear();
            //buffer.Append(bufferContent.Substring(lastProcessedIndex));
            //lastProcessedIndex = 0;


            //string bufferContent2 = buffer.ToString();
            //Match rest_match = this._resting.Match(bufferContent2);
            //if (rest_match.Success)
            //{
            //    //This string starts with (Resting), usually this is a room block while idle with other players in the room??
            //    //i see while idle in the bank with one other player
            //    //string toProcess = bufferContent2.Substring(lastProcessedIndex, rest_match.Index-lastProcessedIndex);
            //    Dictionary<string, string> stats = new Dictionary<string, string>();
            //    stats.Add("Resting", rest_match.Groups[1].Value);
            //    this.m_controller.m_sessionForm.UpdatePlayerStats(stats);

            //    lastProcessedIndex = rest_match.Index + rest_match.Length + 1;
            //}

            //buffer.Clear();
            //buffer.Append(bufferContent2.Substring(lastProcessedIndex));

            //if(buffer.ToString() == " ")
            //{
            //    buffer.Clear();
            //}

            //return this;
        }
    }
}


