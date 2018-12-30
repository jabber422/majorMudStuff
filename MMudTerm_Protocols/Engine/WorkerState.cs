using System.Diagnostics;

namespace MMudTerm_Protocols.Engine
{
    //  State      Action       new State
    //  Stopped -> Start()              = ThreadRunning
    //  Stopped -> Connect() -> Start() = Logon
    //  ThreadRunning -> Connect()      = Logon
    //  Logon -> TermCmd -> script      = Connected
    //  Connected -> TermCmd -> script  = GameMenu
    //  GameMenu -> TermCmd -> script   = EnteringGame
    //  EnteringGame -> TermCmd -> script = InGame

    //the state of the worker thread for the script
    public abstract class WorkerState
    {
        public bool Run = false;

        internal virtual WorkerState Start(Engine eng )
        {
           Trace.WriteLine("Invalid state change (start) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        internal virtual WorkerState Stop(Engine eng)
        {
            Trace.WriteLine("Invalid state change (stop) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        internal virtual WorkerState Connect(Engine eng)
        {
            Trace.WriteLine("Invalid state change (Connect) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        internal virtual WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            if (cmd is TermStringDataCmd)
            { 
                if (MyRegex.HpEquals.IsMatch(cmd as TermStringDataCmd))
                {
                    Trace.WriteLine("Jumping to InGame state due to '[HP=' ");
                    return new WorkerState_InGame();
                }
            }
            return this;
        }

        internal virtual WorkerState Disconnect(Engine engine)
        {
            Trace.WriteLine("Invalid state change (Disconnect) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        internal virtual WorkerState IsConnected(Engine engine)
        {
            Trace.WriteLine("Invalid state change (IsConnected) called in abstract base class WorkerState", this.ToString());
            return this;
        }
    }
}
