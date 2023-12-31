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
        internal ConnObj m_connObj;
        ConcurrentQueue<TermCmd> terminal_term_cmds = new ConcurrentQueue<TermCmd>();
        ConcurrentQueue<TermCmd> state_term_cmds = new ConcurrentQueue<TermCmd>();
        private object _term_q_in_use = new object();
        private object _state_q_in_use = new object();


        string DBG_CAT = "SessionController";

        //read access to our session data object
        internal SessionDataObject SessionData { get { return this.m_SessionData; } }
        internal ConnObj Connection { get { return this.m_connObj; } }
        internal SessionState CurrentState { get { return this.m_currentSessionState; } }

        internal MajorMudBbsGame _gameenv;

        public SessionController(SessionDataObject si, SessionForm sf)
        {
            this.m_SessionData = si;
            this.m_sessionForm = sf;
            this.m_currentSessionState = new SessionStateOffline(null, this);

            //This is the thread that runs the game basically.  It will contantly look for new TermCmd and
            Task.Run(() =>
            {
                while (true) // Replace with a proper condition for stopping
                {
                    
                    if (state_term_cmds.Count > 0)
                    {
                        Queue<TermCmd> cmds = new Queue<TermCmd>();
                        lock (_state_q_in_use)
                        {
                            TermCmd cmd;
                            while(state_term_cmds.TryDequeue(out cmd))
                            {
                                cmds.Enqueue(cmd);
                            }
                        }
                        // Process cmd
                        this.m_currentSessionState = this.m_currentSessionState.HandleCommands(cmds);
                    }
                }
            });

            //This is the thread that runs the games terminal.  It will contantly look for new TermCmd and
            Task.Run(() =>
            {
                while (true) // Replace with a proper condition for stopping
                {

                    if (terminal_term_cmds.Count > 0)
                    {
                        Queue<TermCmd> cmds = new Queue<TermCmd>();
                        lock (_term_q_in_use)
                        {
                            TermCmd cmd;
                            while (terminal_term_cmds.TryDequeue(out cmd))
                            {
                                cmds.Enqueue(cmd);
                            }
                        }
                        // Process cmd
                        this.m_sessionForm.Terminal.HandleCommands(cmds);
                    }
                }
            });
        }

        #region event rcv'r from conn obj
        internal void ConnHandler_Disconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnect Event!", DBG_CAT);
            this.m_currentSessionState = this.m_currentSessionState.Disconnect();
        }

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
        internal void ConnHandler_Rcvr(byte[] buffer)
        {
            temp.Add(buffer);
            if (buffer[0] == 91)
            {
            }
            if (buffer.Length == 0)
                return; //TODO: buffer of zero means a disconnect?

            
            //decode the byte[]
            Queue<TermCmd> cmds = m_decoder.DecodeBuffer(buffer);
            foreach(TermCmd c in cmds) {
                lock (this._term_q_in_use)
                {
                    terminal_term_cmds.Enqueue(c);
                }
                lock (this._state_q_in_use)
                {
                    state_term_cmds.Enqueue(c);
                }
            }
            cmds.Clear();
            
        }
        #endregion

        #region Internals
        #region Internals - commands called from SF
        byte[] StringAsciiNewLineMask = System.Text.Encoding.ASCII.GetBytes(new char[] {'\r'});

        internal void Send(string s)
        {
            Debug.WriteLine("Send | " + s + " |", DBG_CAT);
            this.Send(Encoding.ASCII.GetBytes(s));
        }

        internal void Send(byte[] p)
        {
            if (this.m_connObj != null && this.m_connObj.Connected)
            {
                SocketHandler.Send(this.m_connObj, p);
            }
        }
       
        #endregion
        #endregion
        #region API of SessionView
        internal bool Connect()
        {
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

        #region API for SessionState calls
        internal int DisconnectFromServer()
        {
            if(this.m_currentSessionState is SessionStateOffline) {  return -1; }

            if (this.m_connObj != null) { 
                SocketHandler.Disconnect(this.m_connObj);
                this.m_connObj = null;
            }
            return 1;
        }

        internal int ConnectToServer()
        {
            int result = 0;
            if (this.m_connObj == null)
            {
                Debug.WriteLine("SessionController - ConnterToServer - ConObj is null, making a new one");
                this.m_connObj = new ConnObj(this.m_SessionData.ConnectionInfo.IpA, this.m_SessionData.ConnectionInfo.Port);
                result = 1;
            }

            if (SocketHandler.Connect(this.m_connObj))
            {
                this.m_decoder = new MMudTerm_Protocols.AnsiProtocolCmds.AnsiProtocolDecoder();
                this.m_connObj.Rcvr += new RcvMsgCallback(ConnHandler_Rcvr);
                this.m_connObj.Disconnected += new EventHandler(ConnHandler_Disconnected);
                result = 2;
            }
            else
            {
                this.m_connObj.mySocket.Close();
                this.m_connObj = null;
                result = 3;
            }

            return result;
        }

        #endregion
              
        public void Dispose()
        {
            //this.m_connObj.Disconnect();
            //this.m_currentSessionState.Disconnect();
            //this.m_SessionData.Dispose();
            //this.m_sessionForm.Close();
        }
    }
}
