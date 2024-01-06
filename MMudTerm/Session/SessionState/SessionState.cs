using MMudTerm_Protocols;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace MMudTerm.Session.SessionStateData
{
    internal abstract class SessionState
    {
        internal SessionController m_controller;
        protected string DBG_CAT = "SessionState";
        protected bool IAC_DONE = false;
        protected string state_name = "BaseClass_SessionState";

         internal SessionState(SessionState _state, string state_name)
        {
            this.state_name = state_name;
            
            if (_state != null)
            {
                this.m_controller = _state.m_controller;
                this.m_controller.m_sessionForm.UpdateState(this.state_name);
            }
        }

        //Every state will need to handle any data from the server, this data may cause a state change
        internal abstract SessionState HandleCommands( Queue<TermCmd> cmds);

        
        internal virtual SessionState Connect()
        {
            return this;
        }
        internal virtual SessionState Disconnect()
        {
            if(this.m_controller.m_connObj != null && this.m_controller.m_connObj.Connected)
            {
                this.m_controller.m_connObj.Close();
                this.m_controller.m_connObj = null;
            }
            
            return new SessionStateOffline(this);
        }

        public override string ToString()
        {
            return this.state_name;
        }
    }
}
