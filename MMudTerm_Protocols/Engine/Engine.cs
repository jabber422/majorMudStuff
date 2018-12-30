using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMudTerm_Protocols.Engine
{
    //Uses a connection and a decoder. 
    //Connection rcvr thread should fill up the queue
    //Backgroundworker empties it (and updates the gui when data has changed)
    public class Engine
    {
        //connection to the remote server
        public ConnObj ConnObj;
        //decoder to decode what the server sends
        public ProtocolDecoder Decoder;
        //work thread to process decoded msgs
        public BackgroundWorker WorkerThread;
        //state to control worker
        public WorkerState State;
        //control the worker's activity

        //sync object for incoming cmds
        //public Queue<ProtocolCommandLine> Cmds;

        protected Engine(ConnObj connObj)
        {
            this.ConnObj = connObj;
            this.Decoder = new AnsiProtocolDecoder(this.ConnObj);
            this.State = new WorkerState_Stopped();
            this.WorkerThread = new BackgroundWorker();
            this.WorkerThread.WorkerReportsProgress = true;
            this.WorkerThread.WorkerSupportsCancellation = true;
        }

        //The worker loop that processes incoming messages in the queue, your script implements this
        public void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.WorkerThread.CancellationPending)
            {
                this.Decoder.mre.WaitOne();
                List<TermCmd> cmds = this.Decoder.GetTermCmds();
                foreach(TermCmd cmd in cmds)
                {
                    this.State = this.State.HandleTermCmd(this, cmd);
                }
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

        public void Connect()
        {
            this.State = this.State.Connect(this);
        }

        public void Disconnect()
        {
            this.State = this.State.Disconnect(this);
        }

        public void Send(string v)
        {
            this.ConnObj.Send(ASCIIEncoding.ASCII.GetBytes(v));
        }
    }
}
