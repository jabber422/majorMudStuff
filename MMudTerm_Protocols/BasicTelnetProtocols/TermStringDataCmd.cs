using System.Text;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MMudTerm_Protocols
{
    /// <summary>
    /// A string to be printed to the screen
    /// </summary>
    public class TermStringDataCmd : TermCmd
    {
        public byte[] str;
        public bool IsEcho = false;
        public TermStringDataCmd(List<byte> values)
        {
            str = values.ToArray();
        }

        public TermStringDataCmd(List<byte> values, bool stripLineEndings)
        {
            List<byte> val2 = new List<byte>();
            foreach(byte b in values)
            {
                if (b != (byte)'\r' && b != (byte)'\n')
                {
                    val2.Add(b);
                }
            }
            str = val2.ToArray();
        }

        public override void DoCommand(ITermProtocolCmds terminal)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif

            terminal.AddCharString(str);
        }

        public string GetValue()
        {
            return Encoding.ASCII.GetString(this.str);
        }

        public override string ToString()
        {
            return "[TermStringDataCmd] " + (this.IsEcho ? "Echo" : "") + " >" + this.GetValue() + "<";
        }

        internal void Concat(TermStringDataCmd stringDataCmd)
        {
            this.str = ProtocolDecoder.ConcatBuffers(this.str, stringDataCmd.str);
        }

        internal void Concat(TermCarrigeReturnCmd crCmd)
        {
            this.str = ProtocolDecoder.ConcatBuffers(this.str, crCmd.GetValue());
        }

        internal void Concat(TermNewLineCmd nlCmd)
        {
            this.str = ProtocolDecoder.ConcatBuffers(this.str, nlCmd.GetValue());
        }

        public override bool Equals(object obj)
        {
            if(obj is TermStringDataCmd stringCmd)
            {
                if (this.str.Length != stringCmd.str.Length) return false;

                for(int i =0; i < this.str.Length-1; ++i)
                {
                    if (this.str[i] != stringCmd.str[i]) return false;
                }
                return true;
            }
            return base.Equals(obj);
        }
    }
}
