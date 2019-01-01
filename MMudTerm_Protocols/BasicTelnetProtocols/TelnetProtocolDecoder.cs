using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols
{
    //decodes basic telnet protocols, insert standard here[..|.,]
    public class TelnetProtocolDecoder : ProtocolDecoder
    {
        public TelnetProtocolDecoder(ConnObj connObj) : base(connObj)
        {
        }

        protected override void DecodeBuffer(byte[] buffer)
        {
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
#endif

            throw new NotImplementedException("Currently only using ANSI");
        }

        public override List<ProtocolCommandLine> GetCommandLines()
        {
            lock (this)
            {
                List<ProtocolCommandLine> result = new List<ProtocolCommandLine>();
                if (this.TermCmdsQueue.Count == 0) return result;

                Queue<TermCmd> Fragments = new Queue<TermCmd>();

                while (this.TermCmdsQueue.Count > 1)
                {
                    
                    if(this.TermCmdsQueue.Peek() is TermNewLineCmd)
                    {
                        Fragments.Enqueue(this.TermCmdsQueue.Dequeue());
                        result.Add(new ProtocolCommandLine(Fragments));
                        Fragments = new Queue<TermCmd>();
                    }
                    else
                    {
                        Fragments.Enqueue(this.TermCmdsQueue.Dequeue());
                    }
                }

                //put the leftovers back
                while(Fragments.Count > 0) { 
                    this.TermCmdsQueue.Enqueue(Fragments.Dequeue());
                }

                return result;		

            }
        }

        protected void CreateCommand(TERM_CMD cmd)
        {
            switch (cmd)
            {
                case TERM_CMD.DATA: //aiString += Encoding.ASCII.GetString(values[0]);
                    //TermCmdsQueue.Enqueue(new TermStringDataCmd(values));
                    break;
                case TERM_CMD.NL: TermCmdsQueue.Enqueue(new TermNewLineCmd());
                    break;
                case TERM_CMD.CR: TermCmdsQueue.Enqueue(new TermCarrigeReturnCmd());
                    break;
                default:
#if DEBUG
                    Debug.WriteLine("NYI -- " + cmd.ToString(), this.GetType().Namespace);
#endif
                    break;
            }
            values.Clear();
        }
    }
}