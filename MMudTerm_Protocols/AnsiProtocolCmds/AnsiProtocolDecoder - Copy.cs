using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MMudObjects;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// This will decode ANSI markup strings and convert them to commands
    /// currently used per instance
    /// </summary>
    public class AnsiProtocolDecoderV2 : ProtocolDecoderV2
    {
        AnsiDecoderState _state;
        internal AnsiDecoderState State
        {
            get { return this._state; }
            set { this._state = value; StateChanged(); }
        }

        public AnsiProtocolDecoderV2(ConnObj connObj) : base(connObj)
        {
            this.State = new AnsiDecoderState_LookingForIAC();
        }
        
        public event EventHandler<string> DecoderStateChangeEvent;
 
        private void StateChanged()
        {
            if(this.DecoderStateChangeEvent != null)
            {
                DecoderStateChangeEvent(this, this.State.GetType().ToString());
            }
        }

        //process the buffer from the connobj.rcvr
        protected override void DecodeBuffer(byte[] buffer)
        {
            Log.Enter("Buffer -> " + ASCIIEncoding.ASCII.GetString(buffer) + " <- ");

            Queue<byte> tmpBuffer = new Queue<byte>(buffer);
            while (tmpBuffer.Count != 0)
            {
                this.State = this.State.DoWork(this, tmpBuffer);
            }

            //If the buffer is empty and we are still searching for an iac, dump the rest of the buffer to a string cmd
            if (this.State is AnsiDecoderState_LookingForIAC)
            {
                if ((this.State as AnsiDecoderState_LookingForIAC).HasData)
                {
                    TermCmd cmd = (this.State as AnsiDecoderState_LookingForIAC).GetTermCmd();
                    this.TermCmdsQueue.Enqueue(cmd);
                }
            }

            Log.Exit();
        }
    }
}
