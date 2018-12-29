using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    //the state of the worker thread for the script
    public abstract class WorkerState
    {
        public bool Run = false;

        virtual public WorkerState Start(Engine eng )
        {
           Trace.WriteLine("Invalid state change (start) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        virtual public WorkerState Stop(Engine eng)
        {
            Trace.WriteLine("Invalid state change (stop) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        virtual public WorkerState Connect(Engine eng)
        {
            Trace.WriteLine("Invalid state change (Connect) called in abstract base class WorkerState", this.ToString());
            return this;
        }
    }
}
