using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    internal class WorkerState_InGame : WorkerState
    {
        internal GameProcessorState State;
        internal GameProcessorState PreviousState;
        //internal GameProcessorStateContext StateContext;
        //public Player InGamePlayer { get; internal set; }
        internal Engine Engine;

        //this serves two purposes. 1) have a history of TermCmds for debug
        //  2. concat back to back string cmds, pass that to the GameProcessor state machine
        bool statsRequested = false;

        MatchAndCapture MatchAndCapture;

        //this is the state we will be in most of the time
        //hand off each termcmd to the game Processor state object
        public WorkerState_InGame()
        {
            //InGame player will be null, needs to set us to InitState to trigger the 'stats' cmd and the resulting read.
            //this.State = new GameProcessorState_Init();
            this.State = new GameProcessorState_Matching();
            //this.StateContext = new GameProcessorStateContext();
        }

        List<TermCmd> TermCmdBuffer;
        List<TermCmd> TermCmdBuffer2 = new List<TermCmd>();
        List<TermCmd> TermCmdBuffer3 = new List<TermCmd>();
        List<TermCmd> TermCmdBuffer4 = new List<TermCmd>();

        internal override int FlushCmds()
        {
            if(TermCmdBuffer != null){
                ConsumeGameDataBlock(TermCmdBuffer);
                return TermCmdBuffer.Count;
            }
            return 0;
        }

        //This state is entered when ever the very first [HP= is detected.
        //like the layer below it, this has a thread that polls a queue, this time we queue up CmdTermDataBlock
        //Data blocks are groups of TermCmds that match the known patterns of the game
        //
        internal override WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            //move to ctor/base?  make engine static?
            if (this.Engine == null)
            {
                //cache this, it won't change
                this.Engine = eng;
            }

            if(!statsRequested)
            {
                eng.Send("status\r");
                statsRequested = true;
            }

            if(TermCmdBuffer == null)
            {
                TermCmdBuffer = new List<TermCmd>();
            }

            TermCmdBuffer.Add(cmd);
            if ((cmd is AnsiEraseDisplayCmd) || (cmd is AnsiEraseLineCmd))
            {
                Log.Tag("WorkerState", "Found end of block, sending block data to next level");
                    ConsumeGameDataBlock(TermCmdBuffer);
                    TermCmdBuffer4 = new List<TermCmd>(TermCmdBuffer3);
                    TermCmdBuffer3 = new List<TermCmd>(TermCmdBuffer2);
                    TermCmdBuffer2 = new List<TermCmd>(TermCmdBuffer);
                    TermCmdBuffer = null;
            }

            Log.Tag("WorkerState", "not an end block -> " + cmd.ToString());
            return this;
        }

        private void ConsumeGameDataBlock(List<TermCmd> TermCmdBuffer)
        {
            string msg = "****** Term Cmd DataBlock *******\r\n";
            foreach(TermCmd cmd in TermCmdBuffer)
            {
                if (cmd is TermStringDataCmd stringCmd)
                {
                    if (stringCmd.IsEcho)
                    {
                        msg += "ECHO: ";
                    }
                    msg += stringCmd.ToString() + "\r\n";
                    // msg += stringCmd.GetValue() + "\r\n";
                }
                else
                {
                    msg += cmd.ToString() + "\r\n";
                }
            }
            msg += "******************************************\r\n";
            Log.Tag("Block", msg);

            TermCmdDataBlock block = TermCmdDataBlockFactory.Create(TermCmdBuffer);

            if (block is UnknownTermCmdDataBlock)
            {
                Log.Tag("UnknownCmd", block.cmds[0].ToString());
            }
            else
            {
                //now we have a known TermCmdDataBlock
                block.UpdateModel(this.Engine);
            }
        }
    }

    //public class DataBlockParserState
    //{
    //    List<MatchAndCapture> ThingsThisColorWillMatchAgainst
    //    {
    //        get
    //        {
    //            if (MatchAndCaptureTables.Cache2.ContainsKey(this.CurrentColorAttribute))
    //            {
    //                if (MatchAndCaptureTables.Cache2[this.CurrentColorAttribute].ContainsKey(this.CurrentColorForeGround))
    //                {
    //                    return MatchAndCaptureTables.Cache2[this.CurrentColorAttribute][this.CurrentColorForeGround];
    //                }
    //                else
    //                {
    //                    Log.Tag(this.Tag, "No MatchAndCapture's for Attrib: " + this.CurrentColorAttribute + ", ForeGround: " + this.CurrentColorForeGround);
    //                }
    //            }
    //            else
    //            {
    //                Log.Tag(this.Tag, "No MatchAndCapture's for Attrib: " + this.CurrentColorAttribute);
    //                throw new Exception("Attrib not 0 or 1, need to implement this");
    //            }
    //            return new List<MatchAndCapture>();
    //        }
    //    }

    //    public  HandleTermCmd(TermCmd cmd)
    //    {

    //    }
    //}
}