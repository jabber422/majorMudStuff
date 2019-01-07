using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols
{
    /// <summary>
    /// new line cmd
    /// </summary>
    public class TermNewLineCmd : TermCmd
    {
        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            terminal.DoNewLine();
        }

        public override string ToString()
        {
            return "[TermNewLineCmd]";
        }

        public byte[] GetValue()
        {
            return new byte[] { (byte)'\n' };
        }
    }

    public class TermCarrigeReturnCmd : TermCmd
    {
        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            terminal.DoCarrigeReturn();
        }

        public override string ToString()
        {
            return "[TermCarrigeReturnCmd]";
        }

        public byte[] GetValue()
        {
            return new byte[] { (byte)'\r' };
        }
    }
}
