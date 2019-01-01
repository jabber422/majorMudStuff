using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    class AnsiCursorFwdCmd : TermCmd
    {
        int cols = -1;
        public AnsiCursorFwdCmd(List<byte> vals)
        {
            cols = customAtoi(vals.ToArray());
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            (terminal as IAnsiProtocolCmds).DoCursorFwd(cols);
        }

        public override string ToString()
        {
            return "[AnsiCursorFwdCmd:cols " + this.cols + "]";
        }
    }
}
