using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm.Session
{
    public partial class MacrosUserControl : UserControl
    {
        private SessionController controller;

        public MacrosUserControl()
        {
            InitializeComponent();
        }

        public MacrosUserControl(SessionController controller)
        {
            this.controller = controller;
        }
    }
}
