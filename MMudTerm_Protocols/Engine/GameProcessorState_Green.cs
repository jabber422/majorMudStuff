using MMudObjects;
using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_Green : GameProcessorState
    {
        internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        {
            return this.GetNextState(workerState, cmd);
        }

        internal override void HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd stringCmd)
        {
            Log.Tag(this.Tag, "{0} >" + stringCmd.GetValue() + "<", this.GetType().ToString());

            if(this.IsConsumedValue(workerState, stringCmd))
            {
                Log.Tag(this.Tag, "Consumed Cyan string -> " + stringCmd.GetValue());
                return;
            }

            foreach (AnsiColorRegex acr in AnsiColorRegexTable.Cache[this.GetType()])
            {
                if (!acr.IsKeyMatch(stringCmd)) continue;
                Log.Tag(this.Tag, "Matched - {0} = {1}", acr.KeyPattern, stringCmd.GetValue());
                //the current color is green, we found a familiar string, set this regex so that the next datastring is captured if it's also a match
                // stats keys
                // obvious exits
                // items names when looking at someone
                // names of people in who
                // walks into the room
                // wealth and enc in inventory
                workerState.StateContext.LastRegex = acr;
                if (acr.IsInlineMatch)
                {
                    HandleTermStringDataCmd(workerState, stringCmd);
                }
                return;
            }

            Log.Tag(this.Tag, "Unmatched Green string -> " + stringCmd.GetValue());
        }
    }
}