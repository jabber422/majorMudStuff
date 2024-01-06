namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    class AnsiCursorUpCmd : TermCmd
    {
        int rows = -1;
        public AnsiCursorUpCmd(byte[] vals)
        {
            rows = customAtoi(vals);
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
    }
}
