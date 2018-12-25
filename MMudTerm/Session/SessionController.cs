using System;
using System.Linq;
using System.IO;
using MMudTerm.Connection;
using MMudTerm_Protocols;
using MMudTerm.Session.SessionStateData;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace MMudTerm.Session
{
    internal delegate void StateChangeDel(object sender, object data);
    
    //controler for a specific instance of a connection/character
    public class SessionController : IDisposable
    {
        internal SessionForm m_sessionForm;
        SessionDataObject m_SessionData;
        ProtocolDecoder m_decoder;
        SessionState m_currentSessionState;
        SessionState[] m_states;
        ConnObj m_connObj;

        string DBG_CAT = "SessionController";

        //read access to our session data object
        internal SessionDataObject SessionData { get { return this.m_SessionData; } }
        internal ConnObj Connection { get { return this.m_connObj; } }
        internal SessionStates CurrentState { get { return this.m_currentSessionState.State; } }

        public SessionController(SessionDataObject si, SessionForm sf)
        {
            this.m_SessionData = si;
            this.m_sessionForm = sf;
            InitStates();
        }

        private void InitStates()
        {
            //set up the session state pattern
            this.m_states = new SessionState[Enum.GetNames(typeof(SessionStates)).Length];
            m_states[(byte)SessionStates.OFFLINE] = new SessionStateOffline(this);
            m_states[(byte)SessionStates.CONNECTED] = new SessionStateConnected(this);
            m_states[(byte)SessionStates.LOGON] = new SessionStateLogon(this);
            m_states[(byte)SessionStates.MENU] = new SessionStateMenu(this);
            m_states[(byte)SessionStates.GAME_MENU] = new SessionStateGameMenu(this);
            m_states[(byte)SessionStates.ENTERING_GAME] = new SessionStateEnteringGame(this);
            //m_states[(byte)SessionStates.CHAR_PAGE] = new SessionStateCharPage(this);
            m_states[(byte)SessionStates.IN_GAME] = new SessionStateInGame(this);
            m_states[(byte)SessionStates.MummyScript] = new SessionStateMummyScript(this);

            this.m_currentSessionState = this.m_states[(byte)SessionStates.OFFLINE];
        }

        #region event rcv'r from conn obj
        void ConnHandler_Disconnected(object sender, EventArgs e)
        {
            Debug.WriteLine("Disconnect Event!", DBG_CAT);            
            SetState(SessionStates.OFFLINE);
        }

        //handles the packet rcvd from socket
        void ConnHandler_Rcvr(byte[] buffer)
        {
            if (buffer.Length == 0)
                return; //TODO: buffer of zero means a disconnect?
            //decode the byte[]
            ProtocolCommand cmd = m_decoder.DecodeBuffer(buffer);
            foreach (TermCmd c in cmd.Fragments)
            {
                Queue<TermCmd> sessionsCmds = new Queue<TermCmd>();
                sessionsCmds.Enqueue(c);
                //depends on our state depends on how we process data from the server
                this.m_currentSessionState.HandleCommands(sessionsCmds);
                //send cmds to terminal for drawing
                Queue<TermCmd> termCmds = new Queue<TermCmd>();
                termCmds.Enqueue(c);
                this.m_sessionForm.Terminal.HandleCommands(termCmds);
            }

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
            SessionStates state = this.m_currentSessionState.Connect();
            if (state == SessionStates.CONNECTED)
            {
                result = true;
            }
            this.SetState(state);

            if (this.m_SessionData.LogonEnabled) this.SetState(SessionStates.LOGON);
            else if (this.m_SessionData.EnterGameEnabled) this.SetState(SessionStates.ENTERING_GAME);
            else if (this.m_SessionData.MummyScriptEnabled) this.SetState(SessionStates.MummyScript);
            
            return result;
            
        }

        internal bool Disconnect()
        {
            bool result = false;
            SessionStates state = this.m_currentSessionState.Disconnect();
            if (state == SessionStates.OFFLINE)
            {
                result = true;
            }
            this.SetState(state);
            return result;
        }
        #endregion

        #region API for SessionState calls
        internal int DisconnectFromServer()
        {
            int result = 0;
            if (this.m_connObj == null && this.m_currentSessionState.State == SessionStates.OFFLINE)
                result = 1;
            else if(this.m_connObj != null && this.m_currentSessionState.State != SessionStates.OFFLINE)
                result = 2;
            else
            {
                throw new Exception("Invalid coonObj state and session state!");
            }

            if(result == 2){
                SocketHandler.Disconnect(this.m_connObj);
                this.m_connObj = null;
                SetState(SessionStates.OFFLINE);
                result = 1;
            }
            return result;
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

        internal int SetLogonState()
        {
            SetState(SessionStates.LOGON);
            return 1;
        }

        #endregion
              
        public void Dispose()
        {
            this.m_connObj.Disconnect();
            this.m_currentSessionState.Disconnect();
            this.m_SessionData.Dispose();
            this.m_sessionForm.Close();
        }

        internal void SetState(SessionStates sessionStates)
        {
            Debug.WriteLine("State change FROM: " + this.m_currentSessionState.State.ToString() + 
                " TO: " + sessionStates.ToString(), DBG_CAT);
            this.m_currentSessionState = this.m_states[(byte)sessionStates];
        }

        internal void SetMenuState()
        {
            this.SetState(SessionStates.MENU);
        }

        internal void SetGameMenuState()
        {
            this.SetState(SessionStates.GAME_MENU);
        }

        internal void SetEnteringGameState()
        {
            this.SetState(SessionStates.ENTERING_GAME);
        }

        internal void SetInGameState()
        {
            throw new NotImplementedException();
        }
    }
}
