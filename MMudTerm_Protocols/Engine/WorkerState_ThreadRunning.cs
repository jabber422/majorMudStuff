using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    public class WorkerState_ThreadRunning : WorkerState
    {
        internal override WorkerState Stop(Engine eng)
        {
            this.Run = false;
            
            if(eng.WorkerThread.IsBusy) eng.WorkerThread.CancelAsync();
            if (eng.WorkerThread_React.IsBusy) eng.WorkerThread_React.CancelAsync();

            return new WorkerState_Stopped();
        }

        internal override WorkerState Connect(Engine eng)
        {
            eng.ConnObj.Connect();
            return new WorkerState_Logon();
        }

        internal override int FlushCmds()
        {
            return -1;
        }
    }
}
