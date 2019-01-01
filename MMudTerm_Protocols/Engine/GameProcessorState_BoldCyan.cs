using MMudObjects;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    //Either a room name or a HP/MP value is next
    internal class GameProcessorState_BoldCyan : GameProcessorState
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
                Log.Tag(this.Tag, "Consumed Bold Cyan string -> " + stringCmd.GetValue());
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

            Log.Tag(this.Tag, "Unmatched Bold Cyan string -> " + stringCmd.GetValue());
        }
        //public override GameProcessorState HandleTermCmd(Engine eng, TermCmd cmd)
        //{
        //    if (!(cmd is TermStringDataCmd))
        //    {
        //        if (cmd is AnsiProtocolCmds.AnsiGraphicsCmd)
        //        {
        //            throw new System.Exception("WTF");
        //        }
        //        Log.Tag("GameProcessorState", "Not a String - Change to Idle state - " + cmd.GetType().ToString());
        //        return new GameProcessorState_Idle();
        //    }

        //    string s = cmd.ToString();

        //    Match m = Regex.Match(s, @"^(\d+)$");
        //    if (m.Success)
        //    {
        //        if (eng.Player != null)
        //        {
        //            eng.Player.Health = int.Parse(m.Groups[1].Value);
        //            MMudObjects.Log.Tag("GameProcessorState", "HP= " + m.Groups[1].Value);
        //        }
        //        return new GameProcessorState_Idle();
        //    }

        //    Regex.Match(s, @"^([^0-9])$");
        //    if (m.Success)
        //    {
        //        MMudObjects.Log.Tag("GameProcessorState", "Room= " + m.Groups[1].Value);
        //        return new GameProcessorState_Idle();
        //    }

        //    Log.Tag("GameProcessorState", this.GetType().ToString());
        //    return this;
        //}
    }
}