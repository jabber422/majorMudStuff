using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    //ansi colors
    public enum ANSI_COLOR : int
    {
        All_off = 0,
        Bold = 1,
        Underscore = 4,
        Blink = 5,
        Reverse = 7,
        Concealed = 8,

        Black = 30,
        Red = 31,
        Green = 32,
        Yellow = 33,
        Blue = 34,
        Magenta = 35,
        Cyan = 36,
        White = 37,

        Black_B = 40,
        Red_B = 41,
        Green_B = 42,
        Yellow_B = 43,
        Blue_B = 44,
        Magenta_B = 45,
        Cyan_B = 46,
        White_B = 47,
    }

}
