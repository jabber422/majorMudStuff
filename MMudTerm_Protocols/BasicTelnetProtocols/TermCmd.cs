using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace MMudTerm_Protocols
{
    /// <summary>
    /// Base class for a TermCmd
    /// All classes must implment DoCommand
    /// When the queue is processed, each command is run on the termterminal
    /// </summary>
    public abstract class TermCmd
    {
        public abstract void DoCommand(ITermProtocolCmds terminal);

        //Ansi IAC codes are one by per value.
        //ex. Backwards
        //[0x0b, '[', '7', '9', 'D'
        //  this converts byte[] { '7', '9' } to int 79
        protected int customAtoi(byte[] b)
        {
            switch (b.Length)
            {
                case 1: return (b[0] & 0x0f);
                case 2: return (((b[0] & 0x0f) * 10) + (b[1] & 0x0f));
                case 3: return (((b[0] & 0x0f) * 100) + ((b[1] & 0x0f) *10)+ (b[0] & 0x0f));
                case 4: //random failures? 
                    return 0;
                default: throw new NotSupportedException("CustomAtoi does not support buffers of length: "+ b.Length);
            }
        }

        //REMOVE LATER
        protected const string _dbg = "PROTO_DEBUG";
        protected void DO_DBG(object o)
        {
            Debug.WriteLine(o, _dbg);
        }

        public abstract override string ToString();
    }
}
