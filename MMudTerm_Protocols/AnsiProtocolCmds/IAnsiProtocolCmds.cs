namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// All supported Ansi Escape sequences
    /// </summary>
    public interface IAnsiProtocolCmds : ITermProtocolCmds
    {
        void DoEraseDisplay();
        void DoEraseLine();
        void DoCursorBkwd(int cols);
        void DoCursorFwd(int cols);
        void DoCursorUp(int rows);
        void DoCursorDown(int rows);
        void SetCurGraphics(params int[] vals);
        void SetCursorPosition(int row, int col);
    }
}
