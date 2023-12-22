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
        private RegexMatcher _matcher = null;
        private string pattern_tick = @"\[HP=(\d+)(?:/MA=(\d+))?\]:(?: \((\w+)\))?";
        private string pattern_stat = @"Name: ";
        private string pattern_room = @"Obvious exits: ";
        private string pattern_inv  = @"You are carrying ";
        private Regex _resting = null;
        private StringBuilder buffer = new StringBuilder();

        public SessionStateInGame(SessionState _state) : base(_state, "In Game")
        {
            Dictionary<string, Action<Match, string>> common_patterns = new Dictionary<string, Action<Match, string>>();
            common_patterns.Add(pattern_room, ProcessRoom);
            common_patterns.Add(pattern_stat, ProcessStat);
            common_patterns.Add(pattern_inv, ProcessInventory);
            common_patterns.Add(@"\*Combat ", ProcessCombat);
            common_patterns.Add(@"$top", ProcessTopList);
            common_patterns.Add(@"$who", ProcessWhoList);
            common_patterns.Add(@"You just bought ", ProcessBoughtSomething);
            common_patterns.Add(@"You sold ", ProcessSoldSomething);
            common_patterns.Add(@"There is no exit in that direction!", ProcessBadRoomMove);
            common_patterns.Add(@"You hid ", ProcessHidSomething);
            common_patterns.Add(@"You notice ", ProcessSearch);
            common_patterns.Add(@"Attempting to sneak...", ProcessSneak);
            common_patterns.Add(@"The following items are for sale here:", ProcessForSale);



            this._matcher = new RegexMatcher(common_patterns);
            this._resting = new Regex(@"\((Resting)\)", RegexOptions.Compiled);
        }

        private void ProcessForSale(Match match, string arg2)
        {
            string result = arg2.Split(new string[] { "for sale here:" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
            this.m_controller.m_sessionForm.UpdateForSale(result);
        }

        private void ProcessSneak(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessSearch(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessHidSomething(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessBadRoomMove(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessSoldSomething(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessBoughtSomething(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

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

        

        public SessionState Handle(string incomingString)
        {
            buffer.Append(incomingString);
            string bufferContent = buffer.ToString();
            if(bufferContent.StartsWith(" (Resting) "))
            {
                Dictionary<string, string> stats = new Dictionary<string, string>();
                stats.Add("Resting", "Resting");
                this.m_controller.m_sessionForm.UpdatePlayerStats(stats);
                bufferContent = bufferContent.Substring(" (Resting) ".Length);
            }

            MatchCollection matches = Regex.Matches(bufferContent, pattern_tick);

            int lastProcessedIndex = 0;
            foreach (Match match in matches)
            {
                string toProcess = bufferContent.Substring(lastProcessedIndex, match.Index - lastProcessedIndex);
                Process(toProcess);
                ProcessTick(match,"");
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

            return this;
        }

        //this gets full strings that would appear between two [HP=x/MA=x]: 'ticks'
        private void Process(string data)
        {
            Console.WriteLine("Processing: " + data.Trim());
            Match match = null;
            Action<Match, string> callback = null;
            if(this._matcher.TryMatch(data, out match, out callback))
            {
                callback(match, data);
            }
        }

        //processes the hp/mp block
        private void ProcessTick(Match match, string s)
        {
            Dictionary<string, string> stats = new Dictionary<string, string>();
            stats.Add("Current Hits", match.Groups[1].Value);

            if (match.Groups[2].Success)
            {
                stats.Add("Current Mana", match.Groups[2].Value);
            }

            if (match.Groups[3].Success)
            {
                stats.Add("Resting", match.Groups[3].Value);
            }
            else
            {
                stats.Add("Resting", "No");
            }
            this.m_controller.m_sessionForm.UpdatePlayerStats(stats);
        }

        //processes the player stat block as a whole frame
        private void ProcessStat(Match match, string s)
        {
            string pattern_stat = @"(\S+)(?: Class)?:[ \t]+(\S+)(?: \S+)?";
            Regex r = new Regex(pattern_stat);
            MatchCollection mc = r.Matches(s);

            Dictionary<string, string> stats = new Dictionary<string, string>();
            foreach(Match m in mc)
            {
                if (m.Success)
                {
                    stats.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
            }
            this.m_controller.m_sessionForm.UpdatePlayerStats(stats);
        }

        //handles the block of data that makes up a typical room frame
        private void ProcessRoom(Match m, string s)
        {
            Dictionary<string, string> room_info = new Dictionary<string, string>();
            s = s.Substring(s.IndexOf('\r')+2);

            string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if(lines.Length <2) 
            { 
                //this should never happen
            }

            room_info["exits"] = lines[lines.Length - 1];
            int idx = room_info["exits"].IndexOf('\b');
            while ( idx > 0)
            {
                string before = room_info["exits"].Substring(0, idx - 1);
                string after = room_info["exits"].Substring(idx+1);
                room_info["exits"] = before+ after; 
                idx = room_info["exits"].IndexOf('\b');
            }


            room_info["name"] = lines[0];
            
            room_info["desc"] = "";
            room_info["items"] = "";
            room_info["here"] = "";

            if (lines.Length == 2)
            {
                this.m_controller.m_sessionForm.UpdateRoom(room_info);
            }

            int name_index=0, desc_index=0, exit_index=lines.Length-1, items_index =0, here_index = 0;
            
            for(int i=1; i < lines.Length; ++i)
            {
                string line = lines[i];
                if(line.StartsWith("You notice"))
                {
                    items_index = i;
                }else if (line.StartsWith("Also here:"))
                {
                    here_index = i;
                }
                else if (line.StartsWith("Obvious exits:"))
                {
                    exit_index = i;
                }
            }

            //impl desc capture here


            //items, need to take from items_index to here or exits
            if(items_index > 0)
            {
                int end_index = here_index;
                if(end_index == 0) { end_index = exit_index; }
                
                string desc = "";
                //there should be a description block
                for (int i = items_index; i < end_index; ++i)
                {
                    desc += lines[i];
                }
                room_info["items"] = desc;
            }

            if (here_index > 0)
            {
                string desc = "";
                //there should be a description block
                for (int i = here_index; i < exit_index; ++i)
                {
                    desc += lines[i];
                }
                room_info["here"] = desc;
            }

            this.m_controller.m_sessionForm.UpdateRoom(room_info);
        }

        private void ProcessCombat(Match m,string s)
        {
            bool in_combat = false;
            if (s.Contains("Engaged*"))
            {
                in_combat = true;
            }
            this.m_controller.m_sessionForm.UpdateCombat(in_combat);
        }

        private void ProcessInventory(Match m, string s)
        {
            //            You are carrying 2 silver nobles, 20 copper farthings, padded vest(Torso),   
            //padded boots(Feet), padded helm(Head), padded pants(Legs), padded gloves
            //(Hands), club(Weapon Hand)
            //You have no keys.                                                             
            //Wealth: 40 copper farthings
            //Encumbrance: 556 / 4032 - None[13 %]
            int carry_index = 0, cash_index = 0, enc_index = 0;
            string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (line.StartsWith("You are carrying "))
                {
                    carry_index = i;
                }
                else if (line.StartsWith("Wealth: "))
                {
                    cash_index = i;
                }
                else if (line.StartsWith("Encumbrance: "))
                {
                    enc_index = i;
                }
            }

            Dictionary<string, string> inv =    new Dictionary<string, string>();
            string carry = "";
            for(int i = carry_index; i < cash_index; ++i)
            {
                carry += lines[i];
            }
            inv.Add("carry", carry);
            inv.Add("cash", lines[cash_index]);
            inv.Add("enc", lines[enc_index]);

            this.m_controller.m_sessionForm.UpdateInv(inv); 
        }

        private void ProcessWhoList(Match m, string s)
        {

        }

        private void ProcessTopList(Match m, string s)
        {

        }
    }
}

//This will match a string against many different regex patterns
public class RegexMatcher
{
    Dictionary<Regex, Action<Match, string>> regexPatterns;

    public RegexMatcher(Dictionary<string, Action<Match, string>> common_patterns)
    {
        this.regexPatterns = new Dictionary<Regex, Action<Match, string>>();
        foreach(KeyValuePair<string, Action<Match, string>> kvp in common_patterns)
        {
            Regex r = new Regex(kvp.Key, RegexOptions.Compiled);
            this.regexPatterns.Add(r, kvp.Value);
        }
    }

    public bool TryMatch(string input, out Match match, out Action<Match, string> callback)
    {
        foreach (var r in regexPatterns)
        {
            var regex = r.Key;
            if (regex.IsMatch(input))
            {
                match = regex.Match(input);
                callback = r.Value;
                return true;
            }
        }

        match = null;
        callback = null;
        return false;
    }
}
