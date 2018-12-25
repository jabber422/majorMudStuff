using System;
namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// erases the line from carat to max cols
    /// </summary>
    public class AnsiEraseLineCmd : TermCmd
    {
        public override void DoCommand(ITermProtocolCmds terminal)
        {
            //Console.WriteLine("EraseLine cmd");
            (terminal as IAnsiProtocolCmds).DoEraseLine();
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}
