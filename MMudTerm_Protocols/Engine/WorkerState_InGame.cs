using MMudObjects;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    internal class WorkerState_InGame : WorkerState
    {
        GameProcessorState State;
        internal GameProcessorStateContext StateContext;
        //public Player InGamePlayer { get; internal set; }
        internal Engine Engine;

        //this serves two purposes. 1) have a history of TermCmds for debug
        //  2. concat back to back string cmds, pass that to the GameProcessor state machine
        int LastTenTermCmds_BufferSize = 10;
        List<TermCmd> LastTenTermCmds;

        bool statsRequested = false;

        //this is the state we will be in most of the time
        //hand off each termcmd to the game Processor state object
        public WorkerState_InGame()
        {
            //InGame player will be null, needs to set us to InitState to trigger the 'stats' cmd and the resulting read.
            //this.State = new GameProcessorState_Init();
            this.State = new GameProcessorState_Idle();
            this.StateContext = new GameProcessorStateContext();
        }

        public TermCmd LastTermCmd
        {
            get
            {
                if (this.LastTenTermCmds == null)
                    return null;
                else
                    return this.LastTenTermCmds[this.LastTenTermCmds.Count - 1];
            }
            set
            {
                if (this.LastTenTermCmds == null)
                {
                    this.LastTenTermCmds = new List<TermCmd>();
                }

                if (this.LastTenTermCmds.Count == LastTenTermCmds_BufferSize)
                {
                    this.LastTenTermCmds.RemoveAt(0);
                }
                this.LastTenTermCmds.Add(value);
            }
        }

        //every command that comes in gets stored in LastTermCmd.
        //In the case that LastCmd and cmd both == TermStringDataCmd.  Concat them.
        //this will build string broken up over packets in the underlying layers
        public TermCmd BufferTermCmd(TermCmd cmd)
        {
            TermCmd result = cmd;
            if (this.LastTermCmd == null)
            {
                this.LastTermCmd = cmd;
                return result;
            }

            if ((cmd is TermStringDataCmd) && (this.LastTermCmd.GetType() == cmd.GetType()))
            {
                byte[] buffer1 = (cmd as TermStringDataCmd).str;
                byte[] buffer2 = (this.LastTermCmd as TermStringDataCmd).str;
                byte[] concat = new byte[buffer1.Length + buffer2.Length];
                System.Buffer.BlockCopy(buffer1, 0, concat, 0, buffer1.Length);
                System.Buffer.BlockCopy(buffer2, 0, concat, buffer1.Length, buffer2.Length);

                result = new TermStringDataCmd(new List<byte>(concat));
                this.LastTermCmd = result;
            }

            return result;
        }



        internal override WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            //move to ctor/base?  make engine static?
            if (this.Engine == null)
            {
                //cache this, it won't change
                this.Engine = eng;
            }

            if(this.Engine.Player == null && !statsRequested)
            {
                eng.Send("status\r");
                statsRequested = true;
            }

            //in the 'in game state' getting one TermCmd at a time, need to process these. This will drive the game engine
            this.State = this.State.HandleTermCmd(this, cmd);
            return this;
        }
    }
}