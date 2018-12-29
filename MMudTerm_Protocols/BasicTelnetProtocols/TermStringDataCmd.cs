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
            if (this.GetValue().Contains("32m") ||
                this.GetValue().StartsWith("n") ||
                (this.GetValue().Length <=2 && !(this.GetValue() == "]:") && !(this.GetValue() == ", ") && !(this.GetValue() == "."))
                )
            {
            }
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

        public override string ToString()
        {
            return this.GetValue();
        }
    }
}
