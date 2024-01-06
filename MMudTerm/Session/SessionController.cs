using System;
using System.Linq;
using System.IO;
using MMudTerm.Connection;
using MMudTerm_Protocols;
using MMudTerm.Session.SessionStateData;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using MMudObjects;
using MMudTerm.Game;
using System.Net.Sockets;

namespace MMudTerm.Session
{
    internal delegate void StateChangeDel(object sender, object data);
    
    //controler for a specific instance of a connection/character
    public class SessionController : IDisposable
    {
        internal SessionForm m_sessionForm;
        internal SessionDataObject m_SessionData;
        internal ProtocolDecoder m_decoder;
        SessionState m_currentSessionState; //a thread changes this, be careful
        //SessionState[] m_states;
        internal TcpClient m_connObj;

        private Task terminal_term_cmds_task = null;
        ConcurrentQueue<TermCmd> terminal_term_cmds = new ConcurrentQueue<TermCmd>();
        ManualResetEventSlim terminal_term_cmds_event = new ManualResetEventSlim(false);
        private Task state_term_cmds_task = null;
        ConcurrentQueue<TermCmd> state_term_cmds = new ConcurrentQueue<TermCmd>();
        ManualResetEventSlim state_term_cmds_event = new ManualResetEventSlim(false);
        private object _term_q_in_use = new object();
        private object _state_q_in_use = new object();


        string DBG_CAT = "SessionController";

        //read access to our session data object
        internal SessionDataObject SessionData { get { return this.m_SessionData; } }
        internal SessionState CurrentState { get { return this.m_currentSessionState; } }

        internal MajorMudBbsGame _gameenv;

        public SessionController(SessionDataObject si, SessionForm sf)
        {
            this.m_SessionData = si;
            this.m_sessionForm = sf;
            this.m_currentSessionState = new SessionStateOffline(null, this);
        }

        private Task StartStateQueueWorkerThread(ManualResetEventSlim term_cmds_event, ConcurrentQueue<TermCmd> term_cmds)
        {
            //This is the thread that runs the game basically.  It will constantly look for new TermCmds and 
            return Task.Run(() =>
            {
                while (true) // Replace with a proper condition for stopping
                {
                    term_cmds_event.Wait();
                    //Console.WriteLine("Unlocked");
                    
                    Queue<TermCmd> cmds = new Queue<TermCmd>();
                    TermCmd cmd;
                    while (term_cmds.TryDequeue(out cmd))
                    {
                        cmds.Enqueue(cmd);
                    }

                    // Process cmd
                    this.m_currentSessionState = this.m_currentSessionState.HandleCommands(cmds);

                    // Check if there are more items in the queue
                    if (term_cmds.IsEmpty)
                    {
                        term_cmds_event.Reset();
                    }
                    else
                    {
                        term_cmds_event.Set();
                    }
                }
            });
        }

        private Task StartTerminalQueueWorkerThread(ManualResetEventSlim term_cmds_event, ConcurrentQueue<TermCmd> term_cmds)
        {
            return Task.Run(() =>
            {
                while (true) 
                {
                    term_cmds_event.Wait();
                    //term_cmds_event.Reset();

                    Queue<TermCmd> cmds = new Queue<TermCmd>();
                    TermCmd cmd;
                    while (term_cmds.TryDequeue(out cmd))
                    {
                        cmds.Enqueue(cmd);
                    }

                    this.m_sessionForm.Terminal.HandleCommands(cmds);
                    if (term_cmds.IsEmpty)
                    {
                        term_cmds_event.Reset();
                    }
                    else
                    {
                        term_cmds_event.Set();
                    }
                }
            });
        }

        #region event rcv'r from conn obj

        class FixedSizeList<T> : List<T>
        {
            private readonly int _maxSize;

            public FixedSizeList(int maxSize)
            {
                _maxSize = maxSize;
            }

            public new void Add(T item)
            {
                base.Add(item);
                if (Count > _maxSize)
                {
                    RemoveAt(0); // Removes the oldest item
                }
            }
        }

        FixedSizeList<byte[]> temp = new FixedSizeList<byte[]>(10);
        

        //handles the packet rcvd from socket
        internal async Task ConnHandler_Rcvr()
        {
            byte[] buffer = new byte[1024*4];
            int bytesRead;

            NetworkStream networkStream = this.m_connObj.GetStream();

            while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                byte[] smaller_buffer = new byte[bytesRead];
                Buffer.BlockCopy(buffer, 0, smaller_buffer, 0, bytesRead);
                temp.Add(smaller_buffer);
                Queue<TermCmd> cmds = m_decoder.DecodeBuffer(smaller_buffer);
                //Console.WriteLine(cmds.Count);
                while (cmds.Count > 0)
                {
                    TermCmd c = cmds.Dequeue();
                    terminal_term_cmds.Enqueue(c);
                    state_term_cmds.Enqueue(c);
                }

                this.state_term_cmds_event.Set();
                this.terminal_term_cmds_event.Set();
                buffer = new byte[1024*4];
            }

            Console.WriteLine("Got 0 bytes!, Disconnected!");
        }
        #endregion

        #region Internals
        #region Internals - commands called from SF

        internal void Send(string s)
        {
            Debug.WriteLine("Send | " + s + " |", DBG_CAT);
            this.Send(Encoding.ASCII.GetBytes(s));
        }

        internal void Send(byte[] p)
        {
            if (this.m_connObj != null && this.m_connObj.Connected)
            {
                NetworkStream ns = this.m_connObj.GetStream();
                lock (ns)
                {
                    ns.Write(p, 0, p.Length);
                }
            }
        }
       
        #endregion
        #endregion
        #region API of SessionView
        internal bool Connect()
        {
            if (this.state_term_cmds_task == null)
            {
                this.state_term_cmds_task = StartStateQueueWorkerThread(state_term_cmds_event, state_term_cmds);
            }
            if (this.terminal_term_cmds_task == null) {
                this.terminal_term_cmds_task = StartTerminalQueueWorkerThread(state_term_cmds_event, terminal_term_cmds);
                }

            bool result = false;
            this.m_currentSessionState = this.m_currentSessionState.Connect();
            if(this.m_currentSessionState is SessionStateConnected) { 
                result = true; 
            }
            return result;
            
        }

        internal bool Disconnect()
        {
            bool result = false;
            this.m_currentSessionState = this.m_currentSessionState.Disconnect();
            if (this.m_currentSessionState is SessionStateOffline) { result = true; }
            return result;
        }
        #endregion
              
        public void Dispose()
        {
            this.m_connObj?.Close();
            //this.m_currentSessionState.Disconnect();
            //this.m_SessionData.Dispose();
            //this.m_sessionForm.Close();
        }
    }
}
