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
    }
}