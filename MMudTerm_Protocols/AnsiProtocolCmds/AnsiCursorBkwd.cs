using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// Ansi proto: moves the cursor left x amount of cols
    /// </summary>
    public class AnsiCursorBkwdCmd : TermCmd
    {
        int cols = -1;
        public AnsiCursorBkwdCmd(List<byte> vals)
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
            (terminal as IAnsiProtocolCmds).DoCursorBkwd(cols);
        }

        public override string ToString()
        {
            return "[AnsiCursorBkwdCmd:cols " + this.cols + "]";
        }
    }
}
