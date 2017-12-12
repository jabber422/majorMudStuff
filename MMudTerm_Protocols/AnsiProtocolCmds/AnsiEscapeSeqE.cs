using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// Enum for ansi escape sequences
    /// </summary>
    public enum ANSI_ESC : byte
    {
        Esc = 0x1b,
        OpenCmdBracket = 0x5b,
        BackSpace = 0x08,

        CursorPosition = (byte)'H',
        CursorPositionf = (byte)'f',
        CursorUp = (byte)'A',
        CursorDown = (byte)'B',
        CursorFwd = (byte)'C',
        CursorBkwd = (byte)'D',
        CursorSave = (byte)'s',
        CursorRestore = (byte)'u',
        EraseDisplay = (byte)'J',
        EraseLine = (byte)'K',
        Graphics = (byte)'m',
        Mode = (byte)'h',
        ResetMode = (Byte)'I',
        KeyboardString = (byte)'p',
        noIdea = 0x21,
    }
}
