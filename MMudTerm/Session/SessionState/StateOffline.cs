using MMudTerm.Connection;
using MMudTerm.Session;
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MMudTerm.Session.SessionStateData
{
    //offline, we do nothing when TermCmds are rcv'd.  Return all cmds and the the display consume them.
    internal class SessionStateOffline : SessionState
    {
        public SessionStateOffline(SessionState _state) : base(_state, "Offline")
        {
        }

        public SessionStateOffline(SessionState _state, SessionController controller) : this(_state)
        {
            this.m_controller = controller;
        }

        internal override SessionState HandleCommands( Queue<TermCmd> cmds)
        {
            throw new Exception("We shouldn't have this connected while in the offline state");
        }

        internal override SessionState Connect()
        {
            int result = 0;
            if (this.m_controller.Connection == null)
            {
                Debug.WriteLine("SessionController - ConnterToServer - ConObj is null, making a new one");
                this.m_controller.m_connObj = new ConnObj(this.m_controller.m_SessionData.ConnectionInfo.IpA, this.m_controller.m_SessionData.ConnectionInfo.Port);
                result = 1;
            }

            if (SocketHandler.Connect(this.m_controller.Connection))
            {
                this.m_controller.m_decoder = new MMudTerm_Protocols.AnsiProtocolCmds.AnsiProtocolDecoder();
                this.m_controller.m_connObj.Rcvr += new RcvMsgCallback(this.m_controller.ConnHandler_Rcvr);
                this.m_controller.m_connObj.Disconnected += new EventHandler(this.m_controller.ConnHandler_Disconnected);
                result = 2;
                return new SessionStateConnected(this);
            }
            else
            {
                this.m_controller.m_connObj.mySocket.Close();
                this.m_controller.m_connObj = null;
                result = 3;
            }

            return this;
        }

        internal override SessionState Disconnect()
        {
            return this;
        }
    }

    //handles then first telnet IAC hand-shake, then moves to Logon
    internal class SessionStateConnected : SessionState
    {
        public SessionStateConnected(SessionState _state) : base(_state, "Connected")
        {
        }

        //Try and handle the telnet IAC stuff here if needed
        internal override SessionState HandleCommands(Queue<TermCmd> cmds)
        {
            while (cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermIAC)
                {
                    if (!this.IAC_DONE)
                    {
                        this.m_controller.m_connObj.Send(new byte[] { 255, 253, 3 });
                        this.m_controller.m_connObj.Send(new byte[] { 255, 253, 3 });
                        this.m_controller.m_connObj.Send(new byte[] { 255, 251, 0 });
                        this.IAC_DONE = true;
                    }
                    else
                    {
                        return new SessionStateLogon(this);
                    }
                }

            }
            return this;
        }
    }

    internal class SessionStateMenu : SessionState
    {
        Dictionary<Regex, string> MenuStrings_Regex;
        Dictionary<Regex, bool> MenuSuccess;

        internal bool MenuComplete
        {
            get
            {
                bool result = true;
                foreach (bool b in this.MenuSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        public SessionStateMenu(SessionState _state) : base(_state, "Server Menu")
        {
            this.MenuStrings_Regex = this.m_controller.SessionData.GetMenuDataStrings();
            this.MenuSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MenuStrings_Regex)
            {
                this.MenuSuccess.Add(kvp.Key, false);
            }
        }

        internal override SessionState HandleCommands( Queue<TermCmd> cmds)
        {
            Queue<TermCmd> returnQ = new Queue<TermCmd>();
            while (cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermStringDataCmd)
                {
                    string msg = (c as TermStringDataCmd).GetValue();
                    Console.WriteLine(msg);
                    if (Regex.Match(msg, @"\(N\)onstop, \(Q\)uit, or \(C\)ontinue\?").Success)
                    {
                        this.m_controller.Send("N");
                    }
                    else
                    {
                        foreach (Regex r in this.MenuStrings_Regex.Keys)
                        {
                            Match m = r.Match(msg);
                            if (m.Success)
                            {
                                string rsp = this.MenuStrings_Regex[r];
                                this.m_controller.Send(rsp);
                                this.MenuSuccess[r] = true;
                            }
                        }
                    }
                }
                returnQ.Enqueue(c);
            }
            if (MenuComplete)
            {
                return new SessionStateGameMenu(this);
            }
            return this;
            
        }
    }

    internal class SessionStateGameMenu : SessionState
    {
        Dictionary<Regex, string> MenuStrings_Regex;
        Dictionary<Regex, bool> MMudMenuSuccess;

        internal bool GameMenuComplete
        {
            get
            {
                bool result = true;
                foreach (bool b in this.MMudMenuSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        public SessionStateGameMenu(SessionState _state) : base(_state, "Game Menu")
        {
            this.MenuStrings_Regex = this.m_controller.SessionData.GetMajorMudMenuStrings();
            this.MMudMenuSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MenuStrings_Regex)
            {
                this.MMudMenuSuccess.Add(kvp.Key, false);
            }
        }

        internal override SessionState HandleCommands( Queue<TermCmd> cmds)
        {
            Queue<TermCmd> returnQ = new Queue<TermCmd>();
            while (cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermStringDataCmd)
                {
                    string msg = (c as TermStringDataCmd).GetValue();
                    Console.WriteLine(msg);
                    if (Regex.Match(msg, @"\(N\)onstop, \(Q\)uit, or \(C\)ontinue\?").Success)
                    {
                        this.m_controller.Send("N");
                    }
                    else
                    {
                        foreach (Regex r in this.MenuStrings_Regex.Keys)
                        {
                            Match m = r.Match(msg);
                            if (m.Success)
                            {
                                string rsp = this.MenuStrings_Regex[r];
                                this.m_controller.Send(rsp);
                                this.MMudMenuSuccess[r] = true;
                            }
                        }
                    }
                }
                returnQ.Enqueue(c);
            }
            if (GameMenuComplete)
            {
                if (this.m_controller.SessionData.EnterGameEnabled)
                {
                    this.m_controller.Send("e" + ((char)0x0d).ToString());
                    return new SessionStateInGame(this);
                }
            }
            return this;
        }
    }

}
