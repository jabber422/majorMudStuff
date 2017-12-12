using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using MMudTerm.Session;

namespace MMudTerm
{
    public partial class SessionConfigForm : Form
    {
        string m_addressString, m_portString, m_rowsString, m_colsString, m_nameString;
        short m_port = 0;
        int m_rows= 0, m_cols = 0;
        public SessionConnectionInfo Data { get; private set; }

        //This form is opened from the new or via load
        public SessionConfigForm()
        {
            InitializeComponent();
            addressTextBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;

            m_nameString = this.nameTextBox.Text;
            m_addressString = this.addressTextBox.Text;
            m_portString = this.portTextBox.Text;
            m_rowsString = this.rowsTextBox.Text;
            m_colsString = this.colsTextBox.Text;

            if (m_nameString == string.Empty) { return; }
            if (m_addressString == string.Empty) { return; }
            if (m_portString== string.Empty) { return; }
            if (m_rowsString== string.Empty) { return; }
            if (m_colsString== string.Empty) { return; }
         
            
            IPAddress[] adds = Dns.GetHostAddresses(m_addressString);
            if (adds.Length == 0)
            {
                this.DialogResult = DialogResult.Abort;
                return;
            }else if(adds.Length > 1)
            {
            }

            m_addressString = adds[0].ToString();

            if (!short.TryParse(m_portString, out m_port)) { return; }
            if (!int.TryParse(m_rowsString, out m_rows)) { return; }
            if (!int.TryParse(m_colsString, out m_cols)) { return; }

            this.Data = new SessionConnectionInfo();
            this.Data.Name = m_nameString;
            this.Data.Ip = m_addressString;
            this.Data.Port = m_port;
            this.Data.Rows = m_rows;
            this.Data.Cols = m_cols;
            this.Data.AutoConnect = this.autoConnectCheckBox.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
