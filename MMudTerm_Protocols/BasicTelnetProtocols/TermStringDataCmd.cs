using System.Text;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MMudTerm_Protocols
{
    /// <summary>
    /// A string to be printed to the screen
    /// </summary>
    public class TermStringDataCmd : TermCmd
    {
        byte[] str;
        public TermStringDataCmd(List<byte[]> values)
        {
            str = values[0];
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif

            terminal.AddCharString(str);
        }

        public string GetValue()
        {
            return Encoding.ASCII.GetString(this.str);
        }
    }

    public class TermIAC : TermCmd
    {
        byte[] str;
        public TermIAC(List<byte[]> values)
        {
            str = values[0];
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
            

        }
        public string GetValue()
        {
            return Encoding.ASCII.GetString(this.str);
        }
    }
}
