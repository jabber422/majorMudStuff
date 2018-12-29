using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMudTerm_Protocols.Script
{
    public abstract class ScriptBase
    {
        //connection to the remote server
        public ConnObj m_connObj;
        //decoder to decode what the server sends
        public ProtocolDecoder decoder;
        //work thread to process decoded msgs
        public BackgroundWorker m_workerThread;
        //state to control worker
        public WorkerState State;
        //control the worker's activity
        public ManualResetEvent mre;

        //sync object for incoming cmds
        public Queue<ProtocolCommandLine> Cmds;



        protected ScriptBase(ConnObj connObj)
        {
            this.m_connObj = connObj;
            this.State = new WorkerState_Stopped();
            this.Cmds = new Queue<ProtocolCommandLine>();
            this.mre = new ManualResetEvent(true);
            this.m_workerThread = new BackgroundWorker();
            this.m_workerThread.WorkerReportsProgress = true;
            this.m_workerThread.WorkerSupportsCancellation = true;


        }

        //The worker loop that processes incoming messages in the queue, your script implements this
        public abstract void Worker_DoWork(object sender, DoWorkEventArgs e);

        //rcvr for the connobj.Rcvr event
        public void connObj_Rcvr(byte[] buffer)
        {
            if (this.decoder == null)
            {
                throw new ArgumentNullException("No decoder was created for the script");
            }
            if (buffer.Length == 0) return;

            List<ProtocolCommandLine> cmds = this.decoder.GetCommandLines();
            foreach (ProtocolCommandLine cmd in cmds)
            {
                this.Cmds.Enqueue(cmd);
                mre.Set();
            }
        }

        //start the script
        public void Start()
        {
            this.State = this.State.Start(this);
        }

        //stop the script
        public void Stop()
        {
            this.State = this.State.Stop(this);
        }

        public void Send(string v)
        {
            this.m_connObj.Send(ASCIIEncoding.ASCII.GetBytes(v));
        }
    }

    //the state of the worker thread for the script
    public abstract class WorkerState
    {
        public bool Run = false;

        virtual public WorkerState Start(ScriptBase scriptBase)
        {
           Trace.WriteLine("Invalid state change (start) called in abstract base class WorkerState", this.ToString());
            return this;
        }

        virtual public WorkerState Stop(ScriptBase scriptBase)
        {
            Trace.WriteLine("Invalid state change (stop) called in abstract base class WorkerState", this.ToString());
            return this;
        }
    }

    public class WorkerState_Running : WorkerState
    {
        public override WorkerState Stop(ScriptBase scriptBase)
        {
            this.Run = false;
            scriptBase.m_connObj.Rcvr -= scriptBase.connObj_Rcvr;
            if(scriptBase.m_workerThread.IsBusy) scriptBase.m_workerThread.CancelAsync();

            return new WorkerState_Stopped();
        }
    }

    public class WorkerState_Stopped : WorkerState
    {
        public override WorkerState Start(ScriptBase scriptBase)
        {
            //hook up to the connobj rcvr
            scriptBase.m_connObj.Rcvr += scriptBase.connObj_Rcvr;
            //connect rcvr to decoder
            scriptBase.m_workerThread.DoWork += scriptBase.Worker_DoWork;
            this.Run = true;
            scriptBase.m_workerThread.RunWorkerAsync();

            return new WorkerState_Running();
        }
    }
}
