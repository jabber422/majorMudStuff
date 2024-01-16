
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MMudTerm.Session.SessionStateData
{
    //give a set of msg and responses will attempt to logon and get to the major mud menu
    //when we spot 'move_to_mud_menu_state' in the stream then state change to 
    //  SessionStateGameMenu

    internal class SessionStateLogon : SessionState
    {
        Dictionary<Regex, string> LogonStrings_Regex;
        Dictionary<Regex, bool> LogonSuccess;

        string move_to_mud_menu_state = "[MAJORMUD]:";
        private int _iac_cnd;

        public SessionStateLogon(SessionState _state) : base(_state, "Logon On")
        {
            
            List<Tuple<string,string>> logon_cmds = this.m_controller.SessionData.ConnectionInfo.LogonAutomation;
            this.LogonStrings_Regex = new Dictionary<Regex, string>();
            foreach (Tuple<string,string> tup in logon_cmds)
            {
                string msg = tup.Item1;
                string rsp = tup.Item2;
                this.LogonStrings_Regex.Add(new Regex(msg), rsp + "\r\n");
            }
            this.LogonStrings_Regex.Add(new Regex(@"\(N\)onstop, "), "N");

            this.LogonSuccess = new Dictionary<Regex, bool>();
            foreach (KeyValuePair<Regex, string> kvp in this.LogonStrings_Regex)
            {
                this.LogonSuccess.Add(kvp.Key, false);
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
                    if (this_cmd.Contains(move_to_mud_menu_state))
                    {
                        if (this.m_controller.EnterTheGame)
                        {
                            this.m_controller.SendLine("enter");
                            return new SessionStateInGame(this);
                        }
                        else
                        {
                            Console.WriteLine("Not entering the game");
                        }
                    }else if (this_cmd.Contains("[HP=") || this_cmd.Contains("Obvious exits:")) {
                        Console.WriteLine("In Logon state but see game messages... changing to InGameState");
                        return new SessionStateInGame(this);

                    }

                    //if the logon control is false we can ignore the buffer
                    if (!this.m_controller.m_SessionData.LogonEnabled) { return this; }

                    Console.WriteLine(this_cmd);
                    foreach (Regex r in this.LogonStrings_Regex.Keys)
                    {
                        Match m = r.Match(this_cmd);
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
                        this.m_controller.Send(new byte[] { 255, 253, 3 });
                        this._iac_cnd++;
                    }
                }                
            }

            if (this_cmd != "")
            {
                //foreach (Regex r in this.JumpToInGame.Keys)
                //{
                //    Match m = r.Match(this_cmd);
                //    if (m.Success)
                //    {
                //        return new SessionStateInGame(this);
                //    }
                //}
            }

            
            //return new SessionStateGameMenu(this);
            
            return this;
        }
    }
}
