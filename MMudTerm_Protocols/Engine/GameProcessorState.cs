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

    public abstract class GameProcessorState
    {
        protected string Tag = "GameProcessorState";

        internal abstract GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd);
        internal abstract void HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd stringCmd);

        internal GameProcessorState GetNextState(WorkerState_InGame workerState, TermCmd cmd)
        {
            GameProcessorState nextState;
            if (cmd is TermStringDataCmd)
            {
                this.HandleTermStringDataCmd(workerState, (cmd as TermStringDataCmd));
                nextState = this;
            }
            else if (//ignore these for now
                (cmd is TermCarrigeReturnCmd) || (cmd is TermNewLineCmd) ||
                (cmd is AnsiCursorFwdCmd) || (cmd is AnsiCursorPosition) ||
                (cmd is AnsiCursorUpCmd) || (cmd is AnsiEraseDisplayCmd) ||
                (cmd is AnsiEraseLineCmd) || (cmd is AnsiCursorBkwdCmd)
                )
            {
                nextState = this;
            }
            else if (cmd is AnsiGraphicsCmd)  //only ansi and string is driving the engine
            {
                AnsiGraphicsCmd ansiCmd = (AnsiGraphicsCmd)cmd;

                try
                {
                    foreach (int i in ansiCmd.vals)
                    {
                        if (i == 1 || i == 0)
                        {
                            workerState.StateContext.Attribute = i;
                        }

                        if (i >= 30 && i <= 37)
                        {
                            workerState.StateContext.Foreground = i;
                        }

                        if (i >= 40 && i <= 47)
                        {
                            workerState.StateContext.Background = i;
                        }
                    }

                    if (workerState.StateContext.Attribute == 1)
                    {
                        nextState = GameProcess_AnsiColorStateMap.Bold[workerState.StateContext.Foreground];
                    }
                    else //0
                        nextState = GameProcess_AnsiColorStateMap.Normal[workerState.StateContext.Foreground];
                } catch (Exception ex)
                {
                    Log.Tag(this.Tag, ex.Message);
                    nextState = new GameProcessorState_Idle();
                }
            }
            else
            {
                throw new System.Exception("Unsupported Protocol Cmd!");
            }

            Log.Tag(this.Tag, "In State: {0} -> {1} : Cmd - {2}", this.GetType().Name, nextState.GetType().Name, cmd.ToString());
            return nextState;
        }

        internal bool IsConsumedValue(WorkerState_InGame workerState, TermStringDataCmd stringCmd) {
            if (workerState.StateContext.LastRegex != null)
            {
                if (workerState.StateContext.LastRegex.IsValueMatch(stringCmd))
                {
                    //we are a value for a waiting key
                    workerState.StateContext.LastRegex.UpdateValue(workerState, stringCmd);
                    workerState.StateContext.LastRegex = null;
                    return true;
                }
                //there is a key looking for a  value and this was not the value
                Log.Tag(this.Tag, "Trying to capture a value but failed!\r\n\t/{0}/\r\n\t/{1}/\r\n>{2}<",
                    workerState.StateContext.LastRegex.KeyPattern, workerState.StateContext.LastRegex.ValuePattern, stringCmd.GetValue());

                //for some reason we have a key waiting for a value and the next string cmd it's the key we are looking for
                workerState.StateContext.LastRegex = null;
                return false;
            }
            return false;
        }
    }
}