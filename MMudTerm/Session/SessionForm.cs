using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MMudTerm.Terminal;
using MMudTerm.Session.SessionStateData;

namespace MMudTerm.Session
{
    /// <summary>
    /// The form for a session, MDI child of MMudTerm
    /// 
    /// This file is the "view" that handles all of the input from the session form
    /// </summary>
    public partial class SessionForm : Form
    {
        TerminalWindow m_term;
        SessionController m_controller;
        SessionDataObject m_sessionData;

        Dictionary<int, SessionGameInfo> m_gameInfos;


        internal TerminalWindow Terminal
        { get { return this.m_term; } }

        //internal SessionDataObject SessionData
        //{ get { return this.SessionData; } }

        internal SessionForm(SessionConnectionInfo sciData)
        {
            InitializeComponent();
            this.m_sessionData = new SessionDataObject(sciData);
           
            this.m_controller = new SessionController(this.m_sessionData, this);

            this.m_term = new TerminalWindow(this.m_sessionData);
            InitTermWindow();

            this.toolStripButtonProxy.Checked = this.m_sessionData.ProxyEnabled;
            this.toolStripButtonLogon.Checked = this.m_sessionData.LogonEnabled;
            //this.toolStripButtonMummy.Checked = this.m_sessionData.EnterGameEnabled;
            this.toolStripButtonMummy.Checked = this.m_sessionData.MummyScriptEnabled;

            this.m_gameInfos = new Dictionary<int, SessionGameInfo>();
            SessionGameInfo m_gameInfo = new SessionGameInfo(this.m_controller);
            m_gameInfos.Add(m_gameInfo.GetHashCode(), m_gameInfo);
            m_gameInfo.FormClosing += M_gameInfo_FormClosing;
            m_gameInfo.Show();
        }

        private void M_gameInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (this.m_gameInfos)
            {
                this.m_gameInfos.Remove(sender.GetHashCode());
            }
        }

        private void InitTermWindow()
        {
            this.m_term.Location = new System.Drawing.Point(3, 3);
            this.m_term.Name = "termWindow";
            this.m_term.Size = new System.Drawing.Size(20, 20);
            this.m_term.BorderStyle = BorderStyle.FixedSingle;
            this.m_term.KeyPress += new KeyPressEventHandler(term_KeyPress);
            this.sessionTermContainer.Controls.Add(this.m_term);
            this.sessionTermContainer.AutoSize = true;
            this.m_term.Init();
        }

        List<char> _buffer = new List<char>();
        //if focus is on the session window any key pressed is buffered, enter will send the buffer
        private void term_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool buffer_until_cr = false;
            if (buffer_until_cr)
            {
                if (e.KeyChar == (char)'\r')
                {
                    this._buffer.Add(e.KeyChar);
                    this.m_controller.Send(Encoding.ASCII.GetBytes(this._buffer.ToArray()));
                    this._buffer.Clear();
                }
                else
                {
                    this._buffer.Add(e.KeyChar);
                }
            }
            else
            {
                this.m_controller.Send(new byte[] { (byte)e.KeyChar });
            }
            
            e.Handled = true;
        }

        private void toolStripConnectBtn_Click(object sender, EventArgs e)
        {
            string state = this.toolStripConnectBtn.Text;
            if (state.Equals("Connect"))
            {
                if (this.m_controller.Connect())
                {
                    this.toolStripConnectBtn.Text = "Disconnect";
                    this.toolStripConnectBtn.Checked = true;
                }
            }
            else if (state.Equals("Disconnect"))
            {
                if(this.m_controller.Disconnect())
                {
                    this.toolStripConnectBtn.Text = "Connect";
                    this.toolStripConnectBtn.Checked = false;
                }
            }
        }

        private void SessionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TODO: save data pop-up
            this.m_controller.Dispose();
            this.m_sessionData.Dispose();
            this.m_term.CleanUp();
        }

        internal void HandleDisconnected()
        {
            //throw new NotImplementedException();
            //TODO: does the view need to do anything?
        }

        private void HandleStateOnline(object sender, object data)
        {
            this.toolStripConnectBtn.Text = "Disconnect";
            this.toolStripConnState.Text = "Online";
        }

        private void HandleStateOffline(object sender, object data)
        {
            this.toolStripConnectBtn.Text = "Connect";
            this.toolStripConnState.Text = "Offline";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.m_sessionData.ProxyEnabled = !this.toolStripButtonProxy.Checked;
            this.toolStripButtonProxy.Checked = this.m_sessionData.ProxyEnabled;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.m_sessionData.LogonEnabled = !this.toolStripButtonLogon.Checked;
            this.toolStripButtonLogon.Checked = this.m_sessionData.LogonEnabled;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.m_sessionData.EnterGameEnabled = !this.toolStripButtonEnter.Checked;
            this.toolStripButtonEnter.Checked = this.m_sessionData.EnterGameEnabled;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.m_sessionData.MummyScriptEnabled = !this.toolStripButtonMummy.Checked;
            this.toolStripButtonMummy.Checked = this.m_sessionData.MummyScriptEnabled;

            //if (this.m_sessionData.MummyScriptEnabled)
            //{
            //    this.m_controller.SetState(SessionStates.MummyScript);
            //}
            //else
            //{
            //    this.m_controller.SetState(SessionStates.CONNECTED);
            //}
                
        }

        internal void UpdateState(string state_name)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateState), state_name);
            }
            else
            {
                this.toolStripStatusLabel1.Text = state_name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SessionGameInfo m_gameInfo = new SessionGameInfo(this.m_controller);
            lock (this.m_gameInfos)
            {
                m_gameInfos.Add(m_gameInfo.GetHashCode(), m_gameInfo);
            }
            m_gameInfo.FormClosing += M_gameInfo_FormClosing;
            m_gameInfo.Show();
        }

        internal void UpdatePlayerStats(Dictionary<string, string> stats)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(UpdatePlayerStats), stats);
            }
            else
            {
                if(stats.ContainsKey("Resting"))
                {
                    if (stats["Resting"] == "Resting")
                    {
                        this.toolStripStatusLabel2.Text = "Resting";
                    }else if(stats["Resting"] == "No")
                    {
                        if (this.toolStripStatusLabel2.Text == "Resting")
                        {
                            this.toolStripStatusLabel2.Text = "Idle";
                        }
                    }
                }
            }
        }

        internal void UpdateCombat()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateCombat));
            }
            else
            {
                this.toolStripStatusLabel2.Text = this.m_controller._gameenv._player.InCombat ? "In Combat" : "Idle";
            }
        }


        public void UpdateTick()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateTick));
            }
            else
            {
                if (this.m_controller._gameenv._player.IsResting)
                {
                    this.toolStripStatusLabel2.Text = "Resting";
                }
            }
        }

        public void Update(string token)
        {
            lock (this.m_gameInfos)
            {
                foreach (SessionGameInfo m_gameInfo in this.m_gameInfos.Values)
                {
                    m_gameInfo.Update(token);
                }
            }

            //these are here because this form uses both of them in the status bar
            switch (token)
            {
                case "in_combat":
                    UpdateCombat();
                    break;
                case "tick":
                    UpdateTick();
                    break;
            }
        }
    }
}
