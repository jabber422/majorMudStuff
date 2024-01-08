using System;
using System.Text.RegularExpressions;

namespace MMudObjects
{

    public class RoomExit 
    {
        public string Exit;
        public string ShortName = "";
        public bool IsDoor = false;
        public bool IsOpen = true;
        public bool IsHidden = false;
        public RoomExit(string exit)
        {
            //there are non displayable characters hidden in the exit strings.  I think this n, '<some capital letter>', '\b', o, r, t, h
            char[] str = exit.ToCharArray();
            string result = "";
            foreach (char c in str)
            {
                if (c == '\b')
                {
                    result = result.Remove(result.Length-1);
                }
                else
                {
                    result += c.ToString();
                }
            }

            Match m = Regex.Match(result, @"(closed|open) (?:door|gate) (\S+),?");
            if (m.Success)
            {
                this.IsDoor = true;
                this.IsOpen = m.Groups[1].Value.Trim() == "open" ? true : false;
                result = m.Groups[2].Value.Trim();
            }

            switch (result)
            {
                case "north": this.ShortName = "N"; break;
                case "west": this.ShortName = "W"; break;
                case "south": this.ShortName = "S"; break;
                case "east": this.ShortName = "E"; break;
                case "northeast": this.ShortName = "NE"; break;
                case "northwest": this.ShortName = "NW"; break;
                case "southeast": this.ShortName = "SE"; break;
                case "southwest": this.ShortName = "SW"; break;
                case "up": this.ShortName = "U"; break;
                case "down": this.ShortName = "D"; break;
            }

            this.Exit = result;
        }

        public override string ToString()
        {
            return this.Exit;
        }

        internal void OpenDoor(bool yes)
        {
            this.IsOpen = yes;
        }

        internal bool ExitEquals(string direction)
        {
            if(direction.Length <= 2)
            {
                return direction.ToUpper() == this.ShortName.ToUpper();
            }
            else
            {
                return direction.ToUpper() == this.Exit.ToUpper();
            }
        }       
    }
}