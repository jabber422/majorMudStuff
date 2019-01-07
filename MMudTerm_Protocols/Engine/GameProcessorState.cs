using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    //all the states to handle when in game
    //read each TermCmd, use Ansi esc and text to drive state engine
    //TermCmd is a AnsiColorObject, use the values to predict what the next message should be
    //  on cyan expect '^HP=' or '^MP=' or '^You notice .* or "^You .*" or "^The .*
    // on bright cyan expect '<room name>' + TermCmd

    internal abstract class GameProcessorState
    {
        protected string Tag = "GameProcessorState";
        internal ANSI_COLOR CurrentColorAttribute;
        internal ANSI_COLOR CurrentColorForeGround;
        internal MatchAndCapture MatchAndCapture { get; set; }

        internal GameProcessorState(GameProcessorState previousState = null)
        {
            if (previousState == null) return;
            this.CurrentColorAttribute = previousState.CurrentColorAttribute;
            this.CurrentColorForeGround = previousState.CurrentColorForeGround;
            this.MatchAndCapture = previousState.MatchAndCapture;
        }



        //internal GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        //{
        //    GameProcessorState nextState;
        //    if (//ignore these for now
        //        (cmd is TermCarrigeReturnCmd) || (cmd is TermNewLineCmd) ||
        //        (cmd is AnsiCursorFwdCmd) || (cmd is AnsiCursorPosition) ||
        //        (cmd is AnsiCursorUpCmd) || (cmd is AnsiEraseDisplayCmd) ||
        //        (cmd is AnsiEraseLineCmd) || (cmd is AnsiCursorBkwdCmd) || (cmd is AnsiEraseDisplayCmd)
        //        )
        //    {
        //        nextState = this;
        //    }
        //    else if (cmd is TermStringDataCmd)//matching and a string
        //    {
        //        nextState = HandleTermStringDataCmd(workerState, cmd as TermStringDataCmd);
        //    }
        //    else if (cmd is AnsiGraphicsCmd)
        //    {
        //        nextState = HandleAnsiGraphicsCmd(workerState, cmd as AnsiGraphicsCmd);
        //    }
        //    else
        //    {
        //        throw new Exception("Unhandled TermCmd -> " + cmd.GetType().Name);
        //    }

        //    Log.Tag(this.Tag, "In State: {0} -> {1} : Cmd - {2}", this.GetType().Name, nextState.GetType().Name, cmd.ToString());
        //    return nextState;
        //}

        internal abstract GameProcessorState HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd cmd);
        internal abstract GameProcessorState HandleAnsiGraphicsCmd(WorkerState_InGame workerState, AnsiGraphicsCmd cmd);
    }

}


