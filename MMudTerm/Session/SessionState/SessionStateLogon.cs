using MMudTerm.Connection;
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MMudTerm.Session.SessionStateData
{
    //handles the username password portion of the logon process
    //the m_controller has a DoLogon T/F that needs to be respected
    //"new": 
    //password: 
    internal class SessionStateLogon : SessionState
    {
        Dictionary<Regex, string> LogonStrings_Regex;
        Dictionary<Regex, bool> LogonSuccess;
        Dictionary<Regex, bool> JumpToInGame;

        string move_to_mud_menu_state = "[MAJORMUD]:";
        private int _iac_cnd;

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

        public SessionStateLogon(SessionState _state) : base(_state, "Logon On")
        {
            this.LogonStrings_Regex = this.m_controller.SessionData.GetLogonDataStrings();
            this.LogonSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.LogonStrings_Regex)
            {
                this.LogonSuccess.Add(kvp.Key, false);
            }

            this.JumpToInGame = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.m_controller.SessionData.GetJumpToInGameSessionsStrings())
            {
                this.JumpToInGame.Add(kvp.Key, false);

            }
        }

        internal override SessionState HandleCommands( Queue<TermCmd> cmds)
        {
            Queue<TermCmd> returnQ = new Queue<TermCmd>();
            string this_cmd = "";
            while(cmds.Count > 0)
            {
                TermCmd c = cmds.Dequeue();
                if (c is TermStringDataCmd)
                {
                    string msg = (c as TermStringDataCmd).GetValue();
                    this_cmd += msg;
                    //this state is always trying to change to the mud menu state
                    if (msg == move_to_mud_menu_state)
                    {
                        //covers manual logon
                        //if we ever see '[MAJORMUD]:' switch states
                        return new SessionStateGameMenu(this);
                    }

                    //if the logon control is false we can ignore the buffer
                    if (!this.m_controller.m_SessionData.LogonEnabled) { return this; }

                    Console.WriteLine(msg);
                    foreach (Regex r in this.LogonStrings_Regex.Keys)
                    {
                        Match m = r.Match(msg);
                        if (m.Success)
                        {
                            string rsp = this.LogonStrings_Regex[r];
                            this.m_controller.Send(rsp);
                            this.LogonSuccess[r] = true;
                        }
                    }
                    
                }
                else if (c is TermIAC)
                {

                    if (this._iac_cnd < 3)
                    {
                        this.m_controller.m_connObj.Send(new byte[] { 255, 253, 3 });
                        this._iac_cnd++;
                    }
                }
                
            }

            if (this_cmd != "")
            {
                foreach (Regex r in this.JumpToInGame.Keys)
                {
                    Match m = r.Match(this_cmd);
                    if (m.Success)
                    {
                        return new SessionStateInGame(this);
                    }
                }
            }

            if (LogonComplete)
            {
                return new SessionStateMenu(this);
            }
            return this;
        }
    }
}
