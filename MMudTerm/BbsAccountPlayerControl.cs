using MMudTerm.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm
{
    public partial class BbsAccountPlayerControl : UserControl
    {
        private LogonAutomationMsgRspControl logonAutomationMsgRspControl1;
        private LogonAutomationMsgRspControl logonAutomationMsgRspControl4;
        private LogonAutomationMsgRspControl logonAutomationMsgRspControl3;
        private LogonAutomationMsgRspControl logonAutomationMsgRspControl2;
        private LogonAutomationMsgRspControl logonAutomationMsgRspControl5;
        private MMudTerm mMudTerm;

        //CON,chysasbbs.ddns.net,23^M
        public BbsAccountPlayerControl(MMudTerm mMudTerm)
        {
            this.mMudTerm = mMudTerm;
            InitializeComponent();
            this.groupBox_bbs_account.SuspendLayout();
            this.SuspendLayout();
            // 
            // logonAutomationMsgRspControl1
            // 
            this.logonAutomationMsgRspControl1 = new LogonAutomationMsgRspControl();
            this.logonAutomationMsgRspControl1.IsEnabled = true;
            this.logonAutomationMsgRspControl1.Location = new System.Drawing.Point(98, 40);
            this.logonAutomationMsgRspControl1.Rsp = "%acct1%";
            this.logonAutomationMsgRspControl1.Msg = "\"new\":";
            this.logonAutomationMsgRspControl1.Name = "logonAutomationMsgRspControl1";
            this.logonAutomationMsgRspControl1.TabIndex = 19;
            // 
            // logonAutomationMsgRspControl2
            // 
            this.logonAutomationMsgRspControl2 = new LogonAutomationMsgRspControl();
            this.logonAutomationMsgRspControl2.IsEnabled = true;
            this.logonAutomationMsgRspControl2.Location = new System.Drawing.Point(98, this.logonAutomationMsgRspControl1.Location.Y + 20);
            this.logonAutomationMsgRspControl2.Rsp = "%pw1%";
            this.logonAutomationMsgRspControl2.Msg = "password:";
            this.logonAutomationMsgRspControl2.Name = "logonAutomationMsgRspControl2";            
            this.logonAutomationMsgRspControl2.TabIndex = 20;
            // 
            // logonAutomationMsgRspControl3
            // 
            this.logonAutomationMsgRspControl3 = new LogonAutomationMsgRspControl();
            this.logonAutomationMsgRspControl3.IsEnabled = true;
            this.logonAutomationMsgRspControl3.Location = new System.Drawing.Point(98, this.logonAutomationMsgRspControl2.Location.Y + 20);
            this.logonAutomationMsgRspControl3.Msg = "(TOP)";
            this.logonAutomationMsgRspControl3.Name = "logonAutomationMsgRspControl3";
            this.logonAutomationMsgRspControl3.Rsp = "m\\r\\n";
            
            this.logonAutomationMsgRspControl3.TabIndex = 21;
            // 
            // logonAutomationMsgRspControl4
            // 
            //this.logonAutomationMsgRspControl4 = new LogonAutomationMsgRspControl();
            //this.logonAutomationMsgRspControl4.IsEnabled = true;
            //this.logonAutomationMsgRspControl4.Location = new System.Drawing.Point(98, this.logonAutomationMsgRspControl3.Location.Y + 20);
            //this.logonAutomationMsgRspControl4.Msg = "[MAJORMUD]:";
            //this.logonAutomationMsgRspControl4.Name = "logonAutomationMsgRspControl4";
            //this.logonAutomationMsgRspControl4.Rsp = "e\\r\\n";
            
            //this.logonAutomationMsgRspControl4.TabIndex = 22;

            this.logonAutomationMsgRspControl5 = new LogonAutomationMsgRspControl();
            this.logonAutomationMsgRspControl5.IsEnabled = false;
            this.logonAutomationMsgRspControl5.Location = new System.Drawing.Point(98, this.logonAutomationMsgRspControl3.Location.Y + 20);
            this.logonAutomationMsgRspControl5.Msg = "";
            this.logonAutomationMsgRspControl5.Name = "logonAutomationMsgRspControl5";
            this.logonAutomationMsgRspControl5.Rsp = "";
            
            this.logonAutomationMsgRspControl5.TabIndex = 23;

            this.groupBox_bbs_account.Controls.Add(this.logonAutomationMsgRspControl1);
            this.groupBox_bbs_account.Controls.Add(this.logonAutomationMsgRspControl5);
            this.groupBox_bbs_account.Controls.Add(this.logonAutomationMsgRspControl4);
            this.groupBox_bbs_account.Controls.Add(this.logonAutomationMsgRspControl3);
            this.groupBox_bbs_account.Controls.Add(this.logonAutomationMsgRspControl2);

            int cnt = 5;

            this.Size = new Size(this.Size.Width, cnt * 30);
            this.groupBox_bbs_account.Size = this.Size;

            this.groupBox_bbs_account.ResumeLayout(false);
            this.groupBox_bbs_account.PerformLayout();
            this.ResumeLayout(false);
        }

        public int ID { get; set; }
        
        //by default this is just Character, once we log into the game this can be updated
        public string CharacterGameName { get { return this.groupBox_bbs_account.Text; }
            set { this.groupBox_bbs_account.Text = value; } }

        private void button_connect_Click(object sender, EventArgs e)
        {
            this.ID = this.mMudTerm.CreateNewSession(this.GetData(), this.GetHashCode());
            if(this.ID == -1) { return; } //con failed

            //we need to tell our handler/controller we want to connect, and we need the logon automation info
            foreach (Control c in this.Controls[0].Controls)
            {
                if (c is TextBox) { (c as TextBox).ReadOnly = true; }
                if (c is CheckBox) { (c as CheckBox).Enabled = false; }
                if (c is Button) { (c as Button).Enabled = false; }
                if (c is LogonAutomationMsgRspControl)
                {
                    foreach (Control c2 in (c as LogonAutomationMsgRspControl).Controls)
                    {
                        if (c2 is TextBox) { (c2 as TextBox).Enabled = false; }
                        if (c2 is CheckBox) { (c2 as CheckBox).Enabled = false; }
                    }
                }
            }
        }

        //For every logonAuto control in the bbs account, collect the Msg and Rsp's
        public List<Tuple<string, string>> GetData()
        {
            List<Tuple<string, string>> logon_msg_rsp = new List<Tuple<string, string>>();
            foreach (Control c in this.Controls[0].Controls)
            {
                
                if(c is LogonAutomationMsgRspControl)
                {
                    var ctrl = (c as LogonAutomationMsgRspControl);
                    var data = ctrl.GetMessage();
                    if (data != null) {
                        logon_msg_rsp.Add(data);
                        }
                    
                }
            }

            return logon_msg_rsp;
        }

        public void Unlock()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(Unlock));
            }
            else
            {
                foreach (Control c in this.Controls[0].Controls)
                {
                    if (c is TextBox) { (c as TextBox).ReadOnly = false; }
                    if (c is CheckBox) { (c as CheckBox).Enabled = true; }
                    if (c is Button) { (c as Button).Enabled = true; }
                    if (c is LogonAutomationMsgRspControl)
                    {
                        foreach (Control c2 in (c as LogonAutomationMsgRspControl).Controls)
                        {
                            if (c2 is TextBox) { (c2 as TextBox).Enabled = true; }
                            if (c2 is CheckBox) { (c2 as CheckBox).Enabled = true; }
                        }
                    }
                }
            }
        }
    }
}
