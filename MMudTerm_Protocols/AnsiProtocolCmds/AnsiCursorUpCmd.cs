using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    class AnsiCursorUpCmd : TermCmd
    {
        int rows = -1;
        public AnsiCursorUpCmd(List<byte> vals)
        {
            rows = customAtoi(vals.ToArray());
        }
        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            (terminal as IAnsiProtocolCmds).DoCursorUp(rows);
        }

        public override string ToString()
        {
            return "[AnsiCursorUpCmd:rows " + this.rows + "]";
        }
    }
}
