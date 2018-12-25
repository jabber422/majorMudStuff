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
        List<int> vals;

        public AnsiGraphicsCmd(List<byte[]> values)
        {
            vals = new List<int>();
            foreach (byte[] b in values)
            {
                AddValue(customAtoi(b));
            }
        }

        private void AddValue(int val)
        {
            ANSI_COLOR p = (ANSI_COLOR)val;
            if (p >= ANSI_COLOR.All_off && p <= ANSI_COLOR.Bold)
            {
                vals.Add(val);
            }
            else if (p >= ANSI_COLOR.Black && p <= ANSI_COLOR.White)
            {
                vals.Add(val);
            }
            else if (p >= ANSI_COLOR.Black_B && p <= ANSI_COLOR.White_B)
            {
                vals.Add(val);
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
            return String.Empty;
        }
    }
}
