using MMudObjects;
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
            Log.Tag("WorkerState", "Invalid state change (start) called in abstract base class WorkerState");
            return this;
        }

        internal virtual WorkerState Stop(Engine eng)
        {
            Log.Tag("WorkerState", "Invalid state change (stop) called in abstract base class WorkerState");
            return this;
        }

        internal virtual WorkerState Connect(Engine eng)
        {
            Log.Tag("WorkerState", "Invalid state change (Connect) called in abstract base class WorkerState");
            return this;
        }

        internal virtual WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            //this is used as a catch all, if the state is 'not in game' but we see a [HP= string fly by... somehow we got in game.
            if (this is WorkerState_InGame) return this;

            if (cmd is TermStringDataCmd)
            { 
                if (MyRegex.HpEquals.IsMatch(cmd as TermStringDataCmd))
                {
                    Log.Tag("WorkerState", "Jumping to InGame state due to '[HP=' ");
                    return new WorkerState_InGame();
                }
            }
            return this;
        }

        internal virtual WorkerState Disconnect(Engine engine)
        {
            Log.Tag("WorkerState", "Invalid state change (Disconnect) called in abstract base class WorkerState");
            return this;
        }

        internal virtual WorkerState IsConnected(Engine engine)
        {
            Log.Tag("WorkerState", "Invalid state change (IsConnected) called in abstract base class WorkerState");
            return this;
        }
    }
}
