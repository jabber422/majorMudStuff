using MMudTerm.Session;
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;

namespace MMudTerm.Session.SessionStateData
{
    //offline, we do nothing when TermCmds are rcv'd.  Return all cmds and the the display consume them.
    internal class SessionStateOffline : SessionState
    {
        internal SessionStateOffline(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.OFFLINE;
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
        {
            return cmds;
        }

        internal override SessionStates Connect()
        {
            int result = this.m_controller.ConnectToServer();
            if (result == 2)
            {
                return SessionStates.CONNECTED;
            }
            else if (result == 3)
            {
                return this.m_state;
            }
            else
            {
                throw new Exception("Not possible state error");
            }
        }

        internal override SessionStates Disconnect()
        {
            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new Exception("Invalid State");
        }
    }

    internal class SessionStateConnected : SessionState
    {
        internal SessionStateConnected(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.CONNECTED;
        }

        internal override SessionStates Connect()
        {
            return this.m_state;
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            if (this.m_controller.SetLogonState() == 1)
            {
                return SessionStates.LOGON;
            }

            Debug.WriteLine("Faield to set the logon state to active!", DBG_CAT);
            return this.m_state;
        }
    }


    internal class SessionStateLogon : SessionState
    {
        Dictionary<Regex, string> LogonStrings_Regex;
        Dictionary<Regex, bool> LogonSuccess;

        internal bool LogonComplete
        {
            get
            {
                bool result = true;
                foreach (bool b in this.LogonSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        internal SessionStateLogon(SessionController controller) : base(controller)
        {
            this.m_state = SessionStates.LOGON;
            this.LogonStrings_Regex = controller.SessionData.GetLogonDataStrings();
            this.LogonSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.LogonStrings_Regex)
            {
                this.LogonSuccess.Add(kvp.Key, false);
            }
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
        {
            Queue<TermCmd> returnQ = new Queue<TermCmd>();
            while(cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermStringDataCmd)
                {
                    string msg = (c as TermStringDataCmd).GetValue();
                    Console.WriteLine(msg);
                    foreach (Regex r in this.LogonStrings_Regex.Keys)
                    {
                        Match m = r.Match(msg);
                        if(m.Success)
                        {
                            string rsp = this.LogonStrings_Regex[r];
                            this.m_controller.Send(rsp);
                            this.LogonSuccess[r] = true;
                        }
                    }
                }
                returnQ.Enqueue(c);
            }
            if (LogonComplete)
            {
                this.m_controller.SetMenuState();
            }
            return returnQ;
        }

        internal override SessionStates Connect()
        {
            throw new Exception("Bad state change, can't go from logo to connect");
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            return this.m_state;
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

        internal SessionStateMenu(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.MENU;
            this.MenuStrings_Regex = controller.SessionData.GetMenuDataStrings();
            this.MenuSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MenuStrings_Regex)
            {
                this.MenuSuccess.Add(kvp.Key, false);
            }
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
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
                this.m_controller.SetGameMenuState();
            }
            return returnQ;
        }

        internal override SessionStates Connect()
        {
            throw new NotImplementedException();
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new NotImplementedException();
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

        internal SessionStateGameMenu(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.GAME_MENU;
            this.MenuStrings_Regex = controller.SessionData.GetMajorMudMenuStrings();
            this.MMudMenuSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MenuStrings_Regex)
            {
                this.MMudMenuSuccess.Add(kvp.Key, false);
            }
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
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
                    this.m_controller.SetEnteringGameState();
                }
            }
            return returnQ;
        }

        internal override SessionStates Connect()
        {
            throw new NotImplementedException();
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new NotImplementedException();
        }
    }

    internal class SessionStateEnteringGame : SessionState
    {
        Dictionary<Regex, string> MMudCharCreationStrings_Regex;
        Dictionary<Regex, string> MMudCharPageStrings_Regex;
        Dictionary<Regex, string> MMudEnterGameStrings_Regex;

        Dictionary<Regex, bool> MMudCharCreationSuccess;
        Dictionary<Regex, bool> MMudCharPageSuccess; 
        Dictionary<Regex, bool> MMudEnterGameSuccess;

        internal bool IsCreation
        {
            get
            {
                bool result = true;
                foreach (bool b in this.MMudCharCreationSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        internal bool IsCharPage
        {
            get
            {
                bool result = true;
                foreach (bool b in this.MMudCharPageSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        internal bool IsInGame
        {
            get
            {
                bool result = true;
                foreach (bool b in this.MMudEnterGameSuccess.Values)
                {
                    result &= b;
                }
                return result;
            }
        }

        internal SessionStateEnteringGame(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.ENTERING_GAME;
            this.MMudCharCreationStrings_Regex = controller.SessionData.GetMajorMudCharCreationStrings();
            this.MMudCharCreationSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MMudCharCreationStrings_Regex)
            {
                this.MMudCharCreationSuccess.Add(kvp.Key, false);
            }

            this.MMudCharPageStrings_Regex = controller.SessionData.GetMajorMudCharPageStrings();
            this.MMudCharPageSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MMudCharPageStrings_Regex)
            {
                this.MMudCharPageSuccess.Add(kvp.Key, false);
            }

            this.MMudEnterGameStrings_Regex = controller.SessionData.GetMajorMudEnterGameStrings();
            this.MMudEnterGameSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.MMudEnterGameStrings_Regex)
            {
                this.MMudEnterGameSuccess.Add(kvp.Key, false);
            }
        }

        //hit e at the game menu
        //this states figures out if we go to in game or in char page or in creation
        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
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
                        foreach (Regex r in this.MMudCharCreationStrings_Regex.Keys)
                        {
                            Match m = r.Match(msg);
                            if (m.Success)
                            {
                                string rsp = this.MMudCharCreationStrings_Regex[r];
                                this.m_controller.Send(rsp);
                                this.MMudCharCreationSuccess[r] = true;
                            }
                        }

                        foreach (Regex r in this.MMudCharPageStrings_Regex.Keys)
                        {
                            Match m = r.Match(msg);
                            if (m.Success)
                            {
                                string rsp = this.MMudCharPageStrings_Regex[r];
                                this.m_controller.Send(rsp);
                                this.MMudCharPageSuccess[r] = true;
                            }
                        }

                        foreach (Regex r in this.MMudEnterGameStrings_Regex.Keys)
                        {
                            Match m = r.Match(msg);
                            if (m.Success)
                            {
                                string rsp = this.MMudEnterGameStrings_Regex[r];
                                this.m_controller.Send(rsp);
                                this.MMudEnterGameSuccess[r] = true;
                            }
                        }
                    }
                }
                returnQ.Enqueue(c);
            }

            if (this.IsCreation)
            {
                //do nothing
            }
            else if (this.IsCharPage)
            {
                //do nothing
            }
            else if (this.IsInGame)
            {
                this.m_controller.SetInGameState();
            }
            return returnQ;
        }

        internal override SessionStates Connect()
        {
            throw new NotImplementedException();
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new NotImplementedException();
        }
    }

    internal class SessionStateInGame : SessionState
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

        internal SessionStateInGame(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.IN_GAME;
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
        {
            return cmds;
        }

        internal override SessionStates Connect()
        {
            throw new NotImplementedException();
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new NotImplementedException();
        }
    }

    internal class SessionStateMummyScript : SessionState
    {
        Dictionary<Regex, string> MenuStrings_Regex;
        Dictionary<Regex, bool> MMudMenuSuccess;
        Timer idleTimer = new Timer(5 * 1000);
        Boolean isIdle = true;

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

        internal SessionStateMummyScript(SessionController controller)
            : base(controller)
        {
            this.m_state = SessionStates.MummyScript;
            idleTimer.Elapsed += idleTimer_Elapsed;
        }

        internal override Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
        {
            //dict.Add(new Regex(@"ID\?"), "ID,2,5454");
            //dict.Add(new Regex(@"CON\?"), "CON,dadosbladebbs.dyndns.org,23");


            Queue<TermCmd> returnQ = new Queue<TermCmd>();
            
            while (cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermStringDataCmd)
                {
                    string msg = (c as TermStringDataCmd).GetValue();
                    Console.WriteLine(msg);
                    if (Regex.Match(msg, @"\*Combat Engaged\*").Success)
                    {
                        idleTimer.Stop();
                        isIdle = false;
                        this.m_controller.m_sessionForm.SetCombat(true);
                    }
                    else if (Regex.Match(msg, @"\*Combat Off\*").Success)
                    {
                        idleTimer.Start();
                        isIdle = true;
                        this.m_controller.m_sessionForm.SetCombat(false);
                    }
                    else if (Regex.Match(msg, @"ID\?").Success)
                    {
                        this.m_controller.Send("ID,2,43");
                    }
                }
            }
            return returnQ;
        }

        void idleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.idleTimer.Enabled)
            {
                this.m_controller.Send(e.SignalTime + " " + ((char)0x0d).ToString());
                
                this.m_controller.Send("s" + ((char)0x0d).ToString());
                this.m_controller.Send("n" + ((char)0x0d).ToString());
                this.m_controller.Send("aa mummy" + ((char)0x0d).ToString());
                this.m_controller.Send("aa wight" + ((char)0x0d).ToString());
            }
            else
            {
                Debug.WriteLine(e.SignalTime + "NO");
            }
        }

        internal override SessionStates Connect()
        {
            throw new NotImplementedException();
        }

        internal override SessionStates Disconnect()
        {
            int result = this.m_controller.DisconnectFromServer();
            if (result == 2) return SessionStates.OFFLINE;

            Debug.WriteLine("Disconnect failed?!?", DBG_CAT);

            return this.m_state;
        }

        internal override SessionStates Logon()
        {
            throw new NotImplementedException();
        }
    }


    internal abstract class SessionState
    {
        protected SessionController m_controller;
        protected SessionStates m_state;
        protected string DBG_CAT = "SessionState";

        internal SessionState(SessionController controller)
        {
            this.m_controller = controller;
        }

        internal virtual Queue<TermCmd> HandleCommands(Queue<TermCmd> cmds)
        {
            return cmds;
        }

        internal SessionStates State { get { return this.m_state; } }

        internal abstract SessionStates Connect();
        internal abstract SessionStates Disconnect();
        internal abstract SessionStates Logon();
    }



    internal enum SessionStates : byte
    {
        OFFLINE = 0,
        CONNECTED,
        LOGON,
        MENU,
        GAME_MENU,
        ENTERING_GAME,
        CHAR_PAGE,
        IN_GAME,
        
        MummyScript,

        NULL = 255,

    }
}
