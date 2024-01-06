using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// erases the entire display
    /// </summary>
    public class AnsiEraseDisplayCmd : TermCmd
    {
        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG
            Debug.WriteLine(this.GetType().FullName + " -> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    base.GetType().FullName);
#endif
            (terminal as IAnsiProtocolCmds).DoEraseDisplay();
        }
    }
}
