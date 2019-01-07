using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds

{
    /// <summary>
    /// Graphics command contains
    /// Attribute settings, used inline to set bold or normal color, not supporting the other stuff
    /// foreground color
    /// background color
    /// string
    /// </summary>
    public class AnsiGraphicsCmd : TermCmd
    {
        //You can get this as a list of Ansi graphic values -> int attrib, int forground, int background
        public List<int> vals;
        //Or as a map of bytes 0 -> attrib [n][0]
        public byte[][] bytes;

        public byte[] Attrib { get { return bytes[0]; } }
        public byte[] Forground { get { return bytes[1]; } }
        public byte[] Background{ get { return bytes[2]; } }

        public ANSI_COLOR Attribute { get; private set; }
        public ANSI_COLOR ForeGround { get; private set; }
        public ANSI_COLOR BackGround { get; private set; }

        //should get [byte[, byte], ';', byte[, byte], ';'
        public AnsiGraphicsCmd(List<byte> values)
        {
            vals = new List<int>();
            List<byte> chunks = new List<byte>();
            foreach (byte b in values)
            {
                if (b == 59)
                {
                    vals.Add(customAtoi(chunks.ToArray()));
//                    idx = 0;
                  //  cnt++;
                    chunks.Clear();
                    continue;
                }
                chunks.Add(b);
            }
            vals.Add(customAtoi(chunks.ToArray()));

            foreach(int i in vals)
                SetEnums(i);
        }

        private void SetEnums(int val)
        {
            ANSI_COLOR p = (ANSI_COLOR)val;
            if (p >= ANSI_COLOR.All_off && p <= ANSI_COLOR.Bold)
            {
                this.Attribute = (ANSI_COLOR)p;
            }
            else if (p >= ANSI_COLOR.Black && p <= ANSI_COLOR.White)
            {
                this.ForeGround = (ANSI_COLOR)p;
            }
            else if (p >= ANSI_COLOR.Black_B && p <= ANSI_COLOR.White_B)
            {
                this.BackGround = (ANSI_COLOR)p;
            }
            else
            {
                
                Debug.WriteLine("Not a valid ANSI color/attrib value: " + val);
            }
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            (terminal as IAnsiProtocolCmds).SetCurGraphics(vals.ToArray());
        }

        public override string ToString()
        {
            string s = "";
            foreach (int i in this.vals)
            {
                s += i + ", ";
            }

            s = s.TrimEnd(new char[] { ',', ' ' });
            return "[AnsiGraphicsCmd: " + s + "]";
        }
    }
}
