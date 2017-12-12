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
            this.toolStripButtonMummy.Checked = this.m_sessionData.EnterGameEnabled;
            this.toolStripButtonMummy.Checked = this.m_sessionData.MummyScriptEnabled;
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

        //if focus is on the session window any key pressed goes to the server
        private void term_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.m_controller.Send(Encoding.ASCII.GetBytes(new char[] { e.KeyChar }));
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

            if (this.m_sessionData.MummyScriptEnabled)
            {
                this.m_controller.SetState(SessionStates.MummyScript);
            }
            else
            {
                this.m_controller.SetState(SessionStates.CONNECTED);
            }
                
        }

        public void SetCombat(bool combatEngaged)
        {
            //this.buttonCombatEngaged
            if (combatEngaged)
            {
                this.buttonCombatEngaged.BackColor = Color.Red;
            }
            else
            {
                this.buttonCombatEngaged.BackColor = System.Drawing.SystemColors.Control;
            }
        }
        
    }
}
