//using System.Collections.Generic;
//using System.Text.RegularExpressions;

//namespace MMudTerm_Protocols.Engine
//{
//    internal class WorkerState_Connected : WorkerState
//    {
//        internal override WorkerState Disconnect(Engine engine)
//        {
//            engine.ConnObj.Disconnect();
//            return new WorkerState_ThreadRunning();
//        }

//        internal override WorkerState Stop(Engine eng)
//        {
//            eng.State = eng.State.Disconnect(eng);
//            if(eng.WorkerThread.IsBusy)
//                eng.WorkerThread.CancelAsync();
//            return new WorkerState_Stopped();
//        }

//        internal override WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
//        {
//            //this is the initial state where we've connected to the server.  We really only care about string data
//            //The first string data will tranistion us to WorkerState_LoggingOn
//            if (!(cmd is TermStringDataCmd)) return this;

//            //getting any data moves us to the log


//            //get all the regex's we should be looking for while in the connected state
//            List<RegexTuple> tuples = MyRegex.Cache[this.GetType()];



//        }
//    }
//}