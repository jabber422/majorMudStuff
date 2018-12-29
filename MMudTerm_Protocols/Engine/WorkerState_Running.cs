using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    public class WorkerState_Running : WorkerState
    {
        public override WorkerState Stop(Engine eng)
        {
            this.Run = false;
            
            if(eng.WorkerThread.IsBusy) eng.WorkerThread.CancelAsync();

            return new WorkerState_Stopped();
        }

        public override WorkerState Connect(Engine eng)
        {
            eng.ConnObj.Connect();
            return new WorkerState_Connecting();
        }
    }
}
