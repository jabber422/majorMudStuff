using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudTermProtocols
{
    /// <summary>
    /// echo cmd
    /// </summary>
    public class TermEchoCharCmd : TermCmd
    {
        byte[] chars;
        public TermEchoCharCmd(byte[] buffer)
        {
            chars = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, chars, 0, buffer.Length);
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
            for (int i = 0; i < chars.Length; ++i)
            {
                switch (chars[i])
                {
                    case 0x08: terminal.DoBackSpace();
                        break;
                    case 0x0d: terminal.DoCarrigeReturn();
                        break;
                    case 0x0a: terminal.DoNewLine();
                        break;
                    default: terminal.AddChar((char)chars[i]);
                        break;
                }
            }
        }
    }
}
