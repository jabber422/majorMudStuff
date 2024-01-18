using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm
{
    public partial class BbsControl : UserControl
    {
        private MMudTerm mmtermform;

        public BbsControl(MMudTerm mmtermform)
        {
            InitializeComponent();
            this.mmtermform = mmtermform;
        }

        private void textBox_name_MouseClick(object sender, MouseEventArgs e)
        {
            this.mmtermform.SelectedBbsControl(this);
        }

        internal void SelectedBbsControl(bool selected)
        {
            if (selected)
            {
                this.groupBox1.BackColor = System.Drawing.SystemColors.ControlDark;
            }
            else
            {
                this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            }
        }

        public string Address { get { return this.textBox_address.Text; } }

        public string Port { get { return this.textBox_port.Text; } }
    }
}
