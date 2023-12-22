using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Timers;

namespace MMudTerm.Session.SessionStateData
{
    internal class SessionStateMummyScript : SessionState
    {
        Dictionary<Regex, string> MenuStrings_Regex;
        Dictionary<Regex, bool> MMudMenuSuccess;
        Timer idleTimer = new Timer(5 * 1000);
        Boolean isIdle = true;

        public SessionStateMummyScript(SessionState _state) : base(_state, "Mummy")
        {
            idleTimer.Elapsed += idleTimer_Elapsed;
        }

        internal override SessionState HandleCommands( Queue<TermCmd> cmds)
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
                        this.m_controller.Send("ID,2,42");
                    }
                }
            }
            return this;
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
    }
}
