using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_Capturing : GameProcessorState
    {
        public GameProcessorState_Capturing(GameProcessorState state) : base(state)
        {
            Log.Tag(this.Tag, "New GameProcessorState created -> " + this.GetType().Name);
        }

        internal override GameProcessorState HandleAnsiGraphicsCmd(WorkerState_InGame workerState, AnsiGraphicsCmd cmd)
        {
            //currently dont use color when matching since we are matching against the last state
            Log.Tag(this.Tag, "Got Ansi Graphic Cmd while capturing");
            return this;
        }

        internal override GameProcessorState HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd cmd)
        {
            string stringCmd = cmd.GetValue();
            Log.Tag(this.Tag, "{0} >" + stringCmd + "<", this.GetType().ToString());

            GameProcessorState LastState = workerState.PreviousState;
            if (workerState.State.Equals(this))
            {
            }
            else
            {

            }

            //check if we have something looking to consume from a cyan text block
            if (LastState.MatchAndCapture != null)
            {
                Log.Tag(this.Tag, "StateContext has previous MatchAndcapture");
                ConsumeResults result = LastState.MatchAndCapture.Consume(stringCmd);
                Log.Tag(this.Tag, "Consume result -> " + result.ToString());
                //something has specifically been set to consume the next string
                if (result == ConsumeResults.CaptureComplete)
                {
                    //the object captured what it wanted, this string has been consumed
                    workerState.Engine.GameDataChange(LastState.MatchAndCapture.CreateDataChangeItem());
                    LastState.MatchAndCapture = null;
                    return new GameProcessorState_Matching(this);
                }
                else if (result == ConsumeResults.CaptureFailed_Ignored)
                {
                    //the consumer failed to match but the task is complete... this is used for known ignored items ]:
                    return new GameProcessorState_Matching(this);
                }
                else
                {
                    //match failed and the item is not complete... let it ride
                }
            }
            return this;
        }
    }

}


