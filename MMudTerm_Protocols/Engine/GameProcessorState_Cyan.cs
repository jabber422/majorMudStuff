using MMudObjects;
using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_Cyan : GameProcessorState
    {
        internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        {
            return this.GetNextState(workerState, cmd);
        }

        internal override void HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd stringCmd)
        {
            Log.Tag(this.Tag, "{0} >" + stringCmd.GetValue() + "<", this.GetType().ToString());

            if (IsConsumedValue(workerState, stringCmd))
            {
                Log.Tag(this.Tag, "Consumed Cyan string -> " + stringCmd.GetValue());
                return;
            }

            foreach (AnsiColorRegex acr in AnsiColorRegexTable.Cache[this.GetType()])
            {
                if (!acr.IsKeyMatch(stringCmd)) continue;
                Log.Tag(this.Tag, "Matched - {0} = {1}", acr.KeyPattern, stringCmd.GetValue());
                //when stats comes in this triggers one invoke on the gui thread per stat (is that good/bad?)

                //when we are green and see Name:, supress updates until t/o (todo) or MagicRes: <val>
                workerState.StateContext.LastRegex = acr;
                return;
            }

            Log.Tag(this.Tag, "Unmatched Cyan string -> " + stringCmd.GetValue());
        }
    }
}