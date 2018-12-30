namespace MMudTerm_Protocols.Engine
{
    public class WorkerState_Stopped : WorkerState
    {
        //in stopped state, start the worker thead
        internal override WorkerState Start(Engine eng)
        {
            //connect rcvr to decoder
            eng.WorkerThread.DoWork += eng.Worker_DoWork;
            this.Run = true;
            eng.WorkerThread.RunWorkerAsync();

            return new WorkerState_ThreadRunning();
        }

        //in stopped state.  start the thread, then connect to the remote server
        internal override WorkerState Connect(Engine eng)
        {
            eng.State = this.Start(eng);
            //the work is running, now we can connect
            return eng.State.Connect(eng);
        }
    }
}
