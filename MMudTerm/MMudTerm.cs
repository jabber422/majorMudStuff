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
        Dictionary<BbsControl, Dictionary<int, BbsAccountPlayerControl>> m_bbs_controls;
        BbsControl selected_bbs_control= null;

        public MMudTerm()
        {
            InitializeComponent();
            m_sessions = new Dictionary<int, SessionForm>();
            m_bbs_controls = new Dictionary<BbsControl, Dictionary<int, BbsAccountPlayerControl>>();
            BbsControl bbsControl1 = new BbsControl(this);
            bbsControl1.Location = new System.Drawing.Point(3, 3);
            bbsControl1.Name = "bbsControl1";
            bbsControl1.SelectedBbsControl(true);
            this.selected_bbs_control = bbsControl1;
            bbsControl1.TabIndex = 0;
            this.m_bbs_controls.Add(bbsControl1, new Dictionary<int, BbsAccountPlayerControl>());
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

            int current_char_count = this.m_bbs_controls[this.selected_bbs_control].Count;
            
            control.Location = new System.Drawing.Point(7, 26+(current_char_count *150));
            control.Name = "char" + current_char_count;
            control.TabIndex = 1+ current_char_count;

            this.m_bbs_controls[this.selected_bbs_control].Add(control.GetHashCode(), control);

            this.splitContainer1.Panel2.SuspendLayout();
            this.SuspendLayout();

            this.splitContainer1.Panel2.Controls.Add(control);

            this.splitContainer1.Panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        internal int CreateNewSession(List<Tuple<string, string>> list, int bbs_control_hash = 0)
        {
            SessionConnectionInfo newData = new SessionConnectionInfo();
            
            newData.LogonAutomation = list;
            newData.BbsControlId = bbs_control_hash;
            try
            {
                newData.Ip = this.selected_bbs_control.Address;
                newData.Port = short.Parse(this.selected_bbs_control.Port);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
                return -1;
            }
            catch (FormatException fe)
            {
                MessageBox.Show("Invalid Port Number");
                return -1;
            }
            catch (Exception e)
            {

            }
            

            SessionForm newSessionForm = new SessionForm(newData);
            newSessionForm.FormClosed += NewSessionForm_FormClosed;

            newSessionForm.Show();
            
            this.m_sessions.Add(newSessionForm.GetHashCode(), newSessionForm);

            return newSessionForm.GetHashCode();
        }

        private void NewSessionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SessionForm form = (sender as SessionForm);
            var id_to_remove = form.m_sessionData.ConnectionInfo.BbsControlId;
            foreach(Dictionary<int, BbsAccountPlayerControl> bbs_accounts in this.m_bbs_controls.Values)
            {
                if (bbs_accounts.ContainsKey(id_to_remove))
                {
                    bbs_accounts[id_to_remove].Unlock();
                    //bbs_accounts.Remove(id_to_remove);
                    break;
                }
            }           

            int hash = form.GetHashCode();
            this.m_sessions.Remove(hash);
        }

        private void Button_newbbs_Click(object sender, EventArgs e)
        {
            BbsControl newbbs = new BbsControl(this);
            newbbs.Location = new System.Drawing.Point(3 +(200 * this.m_bbs_controls.Count), 3);
            newbbs.Name = "bbsControl";
            this.m_bbs_controls.Add(newbbs, new Dictionary<int, BbsAccountPlayerControl>());
            SelectedBbsControl(newbbs);
            this.splitContainer1.Panel1.Controls.Add(newbbs);
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(button_new_character);
        }

        internal void SelectedBbsControl(BbsControl sender)
        {
            foreach(BbsControl control in this.m_bbs_controls.Keys)
            {
                bool res = control.Equals(sender);
                if (res) { SelectBbs(sender); }
                control.SelectedBbsControl(res);
            }
        }

        private void SelectBbs(BbsControl sender)
        {
            this.selected_bbs_control = sender;
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(this.button_new_character);
            foreach (BbsAccountPlayerControl control in this.m_bbs_controls[this.selected_bbs_control].Values)
            {
                this.splitContainer1.Panel2.Controls.Add(control);
            }
        }
    }
}
