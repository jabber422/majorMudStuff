using MMudObjects;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    //Either a room name or a HP/MP value is next
    internal class GameProcessorState_BoldCyan : GameProcessorState
    {
        string myBuffer = "";
        internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        {
            return this.GetNextState(workerState, cmd);
        }

        //internal override void HandleTermStringDataCmd(WorkerState_InGame workerState, string stringCmd)
        //{
        //    Log.Tag(this.Tag, "{0} >" + stringCmd + "<", this.GetType().ToString());

        //    //check if we have something looking to consume from a cyan text block
        //    if (workerState.StateContext.MatchAndCapture != null)
        //    {
        //        Log.Tag(this.Tag, "StateContext has previous MatchAndcapture");
        //        bool result = workerState.StateContext.MatchAndCapture.Consume(stringCmd);
        //        //something has specifically been set to consume the next string
        //        if (result && workerState.StateContext.MatchAndCapture.IsComplete)
        //        {
        //            Log.Tag(this.Tag, "Previous MatchAndCapture has completed with this line");
        //            //the object captured what it wanted, this string has been consumed
        //            workerState.Engine.GameDataChange(workerState.StateContext.MatchAndCapture.CreateDataChangeItem());
        //            workerState.StateContext.MatchAndCapture = null;
        //            return;
        //        }
        //        else if (result && !workerState.StateContext.MatchAndCapture.IsComplete)
        //        {
        //            //doesn't trigger
        //        }
        //        else if (!result && !workerState.StateContext.MatchAndCapture.IsComplete)
        //        {
        //            //the consumer failed to match but the task is complete... this is used for known ignored items ]:
        //        }
        //        else
        //        {
        //            //match failed and the item is not complete... let it ride
        //            Log.Tag(this.Tag, "MatchAndCapture is still capturing");
        //        }
        //    }

        //    foreach (MatchAndCapture mac in MatchAndCaptureTables.Cache[this.GetType()])
        //    {
        //        if (mac.IsMatch(stringCmd))
        //        {
        //            if (!mac.Consume(stringCmd))
        //            {
        //                Log.Tag(this.Tag, "Found Match and now capturing next cmd(s)");
        //                workerState.StateContext.MatchAndCapture = mac;
        //                //have a match but it's still looking for more data, save it and move to the next TermCmd;
        //                return;
        //            }
        //            Log.Tag(this.Tag, "Found Match and consumed this string");
        //            //had a match and it consumed what it needed
        //            return;
        //        }
        //        //not a match
        //    }

        //    Log.Tag(this.Tag, "Unmatched string -> " + stringCmd);
        //}
    }
}