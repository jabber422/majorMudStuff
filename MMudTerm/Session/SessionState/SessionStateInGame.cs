﻿using MMudObjects;
using MMudTerm.Game;
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace MMudTerm.Session.SessionStateData
{
    internal class SessionStateInGame : SessionState
    {
        private string pattern_tick = @"\[HP=(\d+)(?:/MA=(\d+))?\]:(?: \((\w+)\))?";
        
        private Regex _resting = null;
        private StringBuilder buffer = new StringBuilder();

        private MajorMudBbsGame _gameenv = null;

        public SessionStateInGame(SessionState _state) : base(_state, "In Game")
        {
            this._gameenv = new MajorMudBbsGame(this.m_controller);
            this.m_controller._gameenv = this._gameenv;
            this._resting = new Regex(@"\((Resting)\)", RegexOptions.Compiled);
        }
        
        //takes a queue of TermCmds and turns it into a string with \r\n preserved...
        internal override SessionState HandleCommands(Queue<TermCmd> cmds)
        {
            string s = "";
            foreach (TermCmd c in cmds)
            {
                if (c is TermStringDataCmd)
                {
                    s += (c as TermStringDataCmd).GetValue();
                }
                else if (c is TermNewLineCmd)
                {
                    s += '\n';
                }
                else if (c is TermCarrigeReturnCmd)
                {
                    s += '\r';
                }
            }
            var new_state = Handle(s);
            return new_state;
        }

        //processes the text block, deals with partial strings, and chunks eveything use HP=X/MA=x as the delimiter
        public SessionState Handle(string incomingString)
        {
            buffer.Append(incomingString);
            string bufferContent = buffer.ToString();
            if(bufferContent.StartsWith(" (Resting) "))
            {
                this._gameenv._player.IsResting = true;
                bufferContent = bufferContent.Substring(" (Resting) ".Length);
            }

            MatchCollection matches = Regex.Matches(bufferContent, pattern_tick);

            int lastProcessedIndex = 0;
            foreach (Match match in matches)
            {
                string toProcess = bufferContent.Substring(lastProcessedIndex, match.Index - lastProcessedIndex);
                this._gameenv.Process(toProcess);
                if (this._gameenv.result != null)
                {
                    this.m_controller.m_sessionForm.Update(this._gameenv.result);
                }
                this._gameenv.ProcessTick(match, "");
                lastProcessedIndex = match.Index + match.Length;
            }

            buffer.Clear();
            buffer.Append(bufferContent.Substring(lastProcessedIndex));
            lastProcessedIndex = 0;


            string bufferContent2 = buffer.ToString();
            Match rest_match = this._resting.Match(bufferContent2);
            if (rest_match.Success)
            {
                //This string starts with (Resting), usually this is a room block while idle with other players in the room??
                //i see while idle in the bank with one other player
                //string toProcess = bufferContent2.Substring(lastProcessedIndex, rest_match.Index-lastProcessedIndex);
                Dictionary<string, string> stats = new Dictionary<string, string>();
                stats.Add("Resting", rest_match.Groups[1].Value);
                this.m_controller.m_sessionForm.UpdatePlayerStats(stats);

                lastProcessedIndex = rest_match.Index + rest_match.Length + 1;
            }

            buffer.Clear();
            buffer.Append(bufferContent2.Substring(lastProcessedIndex));

            if(buffer.ToString() == " ")
            {
                buffer.Clear();
            }

            return this;
        }
    }
}

