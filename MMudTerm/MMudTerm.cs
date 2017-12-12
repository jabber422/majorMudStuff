using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MMudTerm.Session;

namespace MMudTerm
{
    public partial class MMudTerm : Form
    {
        Dictionary<Guid, SessionForm> m_sessions;

        public MMudTerm()
        {
            InitializeComponent();
            m_sessions = new Dictionary<Guid, SessionForm>();
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

        
    }
}
