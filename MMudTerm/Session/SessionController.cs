using System;
using System.Linq;
using System.IO;
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
using static MMudTerm.Session.SessionStateData.SessionStateInGame;
using System.Linq.Expressions;

namespace MMudTerm.Session
{
    internal delegate void StateChangeDel(object sender, object data);
    
    //controler for a specific instance of a connection/character
    public class SessionController : IDisposable
    {
        //the form that displays the Terminal object and represents a single char on a bbs
        internal SessionForm m_sessionForm;
        //ip/port type of info for connecting to the bbs
        internal SessionDataObject m_SessionData;
        //decoder, converts byte's to TerminalIAC commands, raw bytes to ANSI/TELNET commands
        internal ProtocolDecoder m_decoder;
        //the current state of this controller
        //Offline -> 
        SessionState m_currentSessionState; //a thread changes this, be careful
        //Our Socket to the server
        internal TcpClient m_connObj;

        //2 threads, 2 queues and 2 semaphores
        //after the decoder decodes something into IAC object
        //  it will add the object into two queue's
        //  one thread/queue drives the Terminal view
        //  the other thread/queue drives the game processing engine
        private object _term_q_in_use = new object();
        private Task terminal_term_cmds_task = null;
        internal ConcurrentQueue<TermCmd> terminal_term_cmds = new ConcurrentQueue<TermCmd>();
        internal ManualResetEventSlim terminal_term_cmds_event = new ManualResetEventSlim(false);

        private object _state_q_in_use = new object();
        private Task state_term_cmds_task = null;
        ConcurrentQueue<TermCmd> state_term_cmds = new ConcurrentQueue<TermCmd>();
        ManualResetEventSlim state_term_cmds_event = new ManualResetEventSlim(false);

        string DBG_CAT = "SessionController";

        //read access to our session data object
        internal SessionDataObject SessionData { get { return this.m_SessionData; } }
        internal SessionState CurrentState { get { return this.m_currentSessionState; } }

        public bool EnterTheGame { get; internal set; }

        internal MajorMudBbsGame _gameenv;

        public Macros m_macros;

        internal DateTime _exphr_start;

        public SessionController(SessionDataObject si, SessionForm sf)
        {
            this.m_SessionData = si;
            this.m_sessionForm = sf;
            this.m_currentSessionState = new SessionStateOffline(null, this);
            this.m_macros = new Macros();
            //if no input after 10 seconds, send a \r\n
            _exphr_start = DateTime.Now;
            this._gameenv = new MajorMudBbsGame(this);
        }

        

        private Task StartStateQueueWorkerThread(ManualResetEventSlim term_cmds_event, ConcurrentQueue<TermCmd> term_cmds)
        {
            //This is the thread that runs the game basically.  It will constantly look for new TermCmds and 
            return Task.Run(() =>
            {
                try
                {
                    while (this.running) // Replace with a proper condition for stopping
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
                }catch(Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debug.WriteLine("handler Thread");
                    Debug.WriteLine(ex.StackTrace);
                }
                    
            });
        }

        private Task StartTerminalQueueWorkerThread(ManualResetEventSlim term_cmds_event, ConcurrentQueue<TermCmd> term_cmds)
        {
            return Task.Run(() =>
            {
                try
                {
                    while (this.running)
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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    Debug.WriteLine("Terminal Thread");
                    Debug.WriteLine(ex.StackTrace);
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
            try
            {
                while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    byte[] smaller_buffer = new byte[bytesRead];
                    Buffer.BlockCopy(buffer, 0, smaller_buffer, 0, bytesRead);
                    temp.Add(smaller_buffer);
                    Queue<TermCmd> cmds = m_decoder.DecodeBuffer(smaller_buffer);

                    while (cmds.Count > 0)
                    {
                        TermCmd c = cmds.Dequeue();
                        terminal_term_cmds.Enqueue(c);
                        state_term_cmds.Enqueue(c);
                    }

                    this.state_term_cmds_event.Set();
                    this.terminal_term_cmds_event.Set();
                    buffer = new byte[1024 * 4];
                }

                Console.WriteLine("Got 0 bytes!, Disconnected!");
            }catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                Debug.WriteLine("Rcvr Thread");
                Debug.WriteLine(ex.StackTrace);
            }
        }
        #endregion

        #region Internals
        #region Internals - commands called from SF

        public bool user_has_send_blocked = false;
        internal void Send(string s)
        {
            if(this.send_buffer_timer == null)
            {
                this.send_buffer_timer = new System.Timers.Timer(100);
                this.send_buffer_timer.Elapsed += Send_buffer_timer_Elapsed;
            }
            
            this.send_buffer_timer.Stop();
            this.send_buffer.Add(s);
            this.send_buffer_timer.Start();
            
        }

        //this buffers duplicate sequential commands that come from the automation
        //you can see this in the text block when you gain xp and something enters the room
        //both events say, 'check the room' which causes two \r\n \r\n and two room prints
        private void Send_buffer_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.send_buffer.Count == 0) return;
            List<string> buffered_send_list = new List<string>();
            string last = "";
            foreach(string s in this.send_buffer) {
                if (s == last)
                {
                    continue;
                }
                buffered_send_list.Add(s);
                last = s;
            }
            this.send_buffer = new List<string>();

            foreach (string s in buffered_send_list)
            {
                this.Send(Encoding.ASCII.GetBytes(s));
            }
        }

        internal void SendLine(string s = "\r\n")
        {
            if (s.EndsWith("\r\n"))
            {
                this.Send(s);
            }
            else
            {
                this.Send(s + "\r\n");
            }
        }

        internal void Send(byte[] p)
        {
            this._gameenv?.idle_input_timer.Stop();
            this._gameenv?.idle_input_timer.Start();
            if (this.m_connObj != null && this.m_connObj.Connected)
            {
                NetworkStream ns = this.m_connObj.GetStream();
                lock (ns)
                {
                    ns.Write(p, 0, p.Length);
                }
            }
        }

        object send_buffer_timer_state = null;
        System.Timers.Timer send_buffer_timer = null;
        private List<string> send_buffer = new List<string>();

        private bool running;



        #endregion
        #endregion
        #region API of SessionView
        public async Task<bool> ConnectAsync()
        {
            return await Task.Run(() =>
            {
                // Perform intensive calculations
                return Connect();
            });
        }
        internal bool Connect()
        {
            //this needs to be threaded, connection fails lock up the gui thread for a long time
            this.running = true;
            if (this.state_term_cmds_task == null)
            {
                this.state_term_cmds_task = StartStateQueueWorkerThread(state_term_cmds_event, state_term_cmds);
            }
            if (this.terminal_term_cmds_task == null)
            {
                this.terminal_term_cmds_task = StartTerminalQueueWorkerThread(terminal_term_cmds_event, terminal_term_cmds);
            }
            this._gameenv?.idle_input_timer.Start();

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
            if (this.m_currentSessionState is SessionStateOffline)
            {
                this.running = false;
                //need to send a queue of commands, lets the awaits consume them, then let the thread end gracefully
                this.state_term_cmds_task.Wait();
                this.terminal_term_cmds_task.Wait();
                this._gameenv.idle_input_timer.Stop();
                this.send_buffer_timer.Stop();
                result = true;
            }
            return result;
        }
        #endregion

        //sessionForm starts the teardown,               
        public void Dispose()
        {
            try
            {
                this.m_connObj?.Client.Disconnect(true);
            } catch { }
            this.running = false;
            //this.m_currentSessionState.Disconnect();
            //this.m_SessionData.Dispose();
            //this.m_sessionForm.Close();
        }

        internal void PathWalkerFinished()
        {
            this.m_sessionForm.PathWalkerFinished();
        }

        //internal void AddListener(NewGameEventHandler mummyScriptHandler)
        //{
        //    if(this.CurrentState is SessionStateInGame)
        //    {
        //        (this.CurrentState as SessionStateInGame).NewGameEvent += mummyScriptHandler;
        //    }
        //}



        //internal void RemoveListender(NewGameEventHandler mummyScriptHandler)
        //{
        //    if (this.CurrentState is SessionStateInGame)
        //    {
        //        (this.CurrentState as SessionStateInGame).NewGameEvent -= mummyScriptHandler;
        //    }
        //}
    }
}
