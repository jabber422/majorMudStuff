using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MMudTerm_Protocols.Engine
{
    //Uses a connection and a decoder. 
    //Connection rcvr thread should fill up the queue
    //Backgroundworker empties it (and updates the gui when data has changed)
    public class Engine
    {
        //the player for this session
        public Player Player;
        //connection to the remote server
        public ConnObj ConnObj;
        //decoder to decode what the server sends
        public ProtocolDecoderV2 Decoder;
        //work thread to process decoded msgs
        public BackgroundWorker WorkerThread;

        //reacts to decoded messages
        public BackgroundWorker WorkerThread_React;
        Queue<DataChangeItem> ReactionQueue;
        AutoResetEvent are;

        public delegate void DataUpdateEventHandler(List<string> targetProperties);
        public event DataUpdateEventHandler DataUpdateEvent;

        //state to control worker
        WorkerState _state;
        public WorkerState State
        {
            get { return this._state; }
            set { this._state = value; StateChanged(); }
        }

        public event EventHandler<string> EngineStateChangeEvent;

        public Engine(ConnObj connObj)
        {
            this.ConnObj = connObj;
            this.Decoder = new AnsiProtocolDecoderV2(this.ConnObj);
            this.State = new WorkerState_Stopped();
            this.WorkerThread = new BackgroundWorker();
            this.WorkerThread.WorkerReportsProgress = true;
            this.WorkerThread.WorkerSupportsCancellation = true;

            this.ReactionQueue = new Queue<DataChangeItem>();
            this.WorkerThread_React = new BackgroundWorker();
            this.WorkerThread_React.WorkerReportsProgress = true;
            this.WorkerThread_React.WorkerSupportsCancellation = true;
            this.are = new AutoResetEvent(true);
        }

        private void StateChanged()
        {
            if (this.EngineStateChangeEvent != null)
            {
                EngineStateChangeEvent(this, this.State.GetType().ToString());
            }
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

        //the loop that reacts to data changes in the model
        public void Worker_React_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!this.WorkerThread.CancellationPending)
            {
                this.are.WaitOne();
                List<string> NewDataItems = new List<string>();
                while(ReactionQueue.Count > 0)
                {
                    DataChangeItem dci;
                    lock (ReactionQueue)
                    {
                        dci = ReactionQueue.Dequeue();
                    }

                    if(this.Player == null)
                    {
                        if(dci.targetProperty == "Player.Stats.Name")
                        {
                            this.Player = new Player(dci.groups[1].Value);
                            this.Send("who");
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (dci.targetProperty.StartsWith("Player.Stats"))
                    {
                        this.Player.Stats.Update(dci);
                    }else if (dci.targetProperty.StartsWith("Player.Room"))
                    {
                        this.Player.Room.Update(dci);
                    }
                    else if (dci.targetProperty.StartsWith("Ignored"))
                    {
                        //ignore these expected tags
                    }
                    else
                    {
                        //impl
                    }

                    //just send the whole dci?  The value fields are raw and unparsed, consumer would need to impl
                    //that or they use the string to go read the property from the model
                    NewDataItems.Add(dci.targetProperty);
                }

                //it's possible the connObj.rcvr thread will fill up the termCmd Queue that the work thread works against.
                //the work thread never goes idle becaseu it can't consume the queue fast enough
                //in turn that work thread generates Dci objects and fille up the dci queue faster than this thread can consume them
                //the update would never trigger and the gui is f'd

                //it could happen, will it?  Maybe a timer/counter on the queue to break the loop on intervals?
                Log.Tag("Engine", "Reaction queue is empty, telling listeners and going to sleep");
                if (this.DataUpdateEvent != null && NewDataItems.Count > 0)
                {
                    this.DataUpdateEvent(NewDataItems);
                }
            }
        }


        //start the script
        public void Start()
        {
            this.State = this.State.Start(this);
        }

        //called by the WorkerThread to signal new Player/world data has been updated.
        internal void GameDataChange(string targetProperty, GroupCollection groups)
        {
            DataChangeItem item = new DataChangeItem(targetProperty, groups);
            lock (ReactionQueue)
            {
                ReactionQueue.Enqueue(item);
            }

            are.Set();
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
