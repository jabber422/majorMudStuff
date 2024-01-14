using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using MMudTerm.Session;

namespace MMudTerm
{
    public partial class MMudTerm : Form
    {
        Dictionary<int, SessionForm> m_sessions;
        Dictionary<int, BbsAccountPlayerControl> m_bbs_chars;
        List<BbsControl> m_bbs_controls = new List<BbsControl>();

        public MMudTerm()
        {
            InitializeComponent();
            m_sessions = new Dictionary<int, SessionForm>();
            m_bbs_chars = new Dictionary<int, BbsAccountPlayerControl>();
            BbsControl bbsControl1 = new BbsControl();
            bbsControl1.Location = new System.Drawing.Point(3, 3);
            bbsControl1.Name = "bbsControl1";
            bbsControl1.Size = new System.Drawing.Size(300, 160);
            bbsControl1.TabIndex = 0;
            this.m_bbs_controls.Add(bbsControl1);
            this.splitContainer1.SuspendLayout();
            this.splitContainer1.Panel1.Controls.Add(bbsControl1);
            this.splitContainer1.ResumeLayout(false);
            this.PerformLayout();
        }

        #region widget handlers
        private void newSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SessionConfigForm sessionCfgForm = new SessionConfigForm();
            DialogResult dr = sessionCfgForm.ShowDialog();

            if (dr != DialogResult.OK)
            {
                MessageBox.Show("Sessiongconfig Form closed without an OK!");
                return;
            }

            SessionConnectionInfo newData = sessionCfgForm.Data;
            SessionForm newSessionForm = new SessionForm(newData);
            newSessionForm.Show();
        }

        private void loadSessionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult dr = ofd.ShowDialog();

            if (dr != DialogResult.OK)
            {
                MessageBox.Show("Sessiongconfig Form closed without an OK!");
                return;
            }
        }
        #endregion

        #region widget call implmentation
        private void CreateSession()
        {
        }
        #endregion

        //move to central util
        private bool CheckIsEmptyString(string addressString)
        {
            if (addressString.Equals(string.Empty))
            {
                return true;
            }

            return false;
        }

        private void button_new_char_Click(object sender, EventArgs e)
        {
            BbsAccountPlayerControl control = new BbsAccountPlayerControl(this);

            int current_char_count = this.m_bbs_chars.Count;
            
            control.Location = new System.Drawing.Point(7, 26+(current_char_count *150));
            control.Name = "char" + current_char_count;
            control.TabIndex = 1+ current_char_count;

            this.m_bbs_chars.Add(control.GetHashCode(), control);

            this.groupBox_chars.SuspendLayout();
            this.SuspendLayout();

            this.groupBox_chars.Controls.Add(control);

            this.groupBox_chars.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        internal int CreateNewSession(List<Tuple<string, string>> list, int bbs_control_hash = 0)
        {
            SessionConnectionInfo newData = new SessionConnectionInfo();
            //try
            //{
            //    newData.Ip = this.textBox_address.Text;
            //    newData.Port = short.Parse(this.textBox_port.Text);
            //}
            //catch (SocketException ex)
            //{
            //    MessageBox.Show("Bad IP!\r\n" + ex.Message);
            //    this.textBox_address.Focus();
            //    this.textBox_address.SelectAll();
            //    return -1;
            //}catch(FormatException ex)
            //{
            //    MessageBox.Show("Bad Port!\r\n" + ex.Message);
            //    this.textBox_port.Focus();
            //    this.textBox_port.SelectAll();
            //    return -1;
            //}
            
            newData.LogonAutomation = list;
            newData.BbsControlId = bbs_control_hash;

            SessionForm newSessionForm = new SessionForm(newData);
            newSessionForm.FormClosed += NewSessionForm_FormClosed;

            newSessionForm.Show();
            
            this.m_sessions.Add(newSessionForm.GetHashCode(), newSessionForm);

            return newSessionForm.GetHashCode();
        }

        private void NewSessionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SessionForm form = (sender as SessionForm);
            BbsAccountPlayerControl control = this.m_bbs_chars[form.m_sessionData.ConnectionInfo.BbsControlId];
            control.Unlock();

            int hash = form.GetHashCode();
            this.m_sessions.Remove(hash);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BbsControl newbbs = new BbsControl();
            newbbs.Location = new System.Drawing.Point(3 +(300 * this.m_bbs_controls.Count), 3);
            newbbs.Name = "bbsControl";
            newbbs.Size = new System.Drawing.Size(300, 160);
            this.m_bbs_controls.Add(newbbs);
            this.splitContainer1.Panel1.Controls.Add(newbbs);
        }
    }
}
