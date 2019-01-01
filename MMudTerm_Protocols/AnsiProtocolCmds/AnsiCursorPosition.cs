using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    internal class AnsiCursorPosition : TermCmd
    {
        int row, col;
        internal AnsiCursorPosition(List<byte> values)
        {
            if (values.Count != 2)
                throw new Exception("AnsiCursorPosition should only have 2 values");
            int idx = values.IndexOf((byte)';');
            if(idx == 0)
                throw new Exception("AnsiCursorPosition must have 2 values");

            col = customAtoi(values.GetRange(0, idx).ToArray());
            row = customAtoi(values.GetRange(idx, values.Count - idx).ToArray());
        }
        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif
            (terminal as IAnsiProtocolCmds).SetCursorPosition(row, col);
        }

        public override string ToString()
        {
            return "[AnsiCursorPositionCmd:cols " + this.col + ":rows "+ this.row + "]";
        }
    }
}
