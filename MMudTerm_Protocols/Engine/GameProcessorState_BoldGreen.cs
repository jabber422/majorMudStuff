using MMudObjects;
using System.Diagnostics;
using MMudObjects;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_BoldGreen : GameProcessorState
    {
        internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        {
            return this.GetNextState(workerState, cmd);
        }
    }
}