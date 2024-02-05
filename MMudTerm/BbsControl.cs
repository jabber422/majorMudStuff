using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm
{
    public partial class BbsControl : UserControl
    {
        private MMudTerm mmtermform;
        private bool changed;

        public BbsControl(MMudTerm mmtermform)
        {
            InitializeComponent();
            this.mmtermform = mmtermform;
            this.button_save.Enabled = false;
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

        public string BbsName { get { return textBox_name.Text.Trim(); } }

        public BbsControlData SaveData()
        {
            var data = new BbsControlData();
            data.Name = BbsName;
            data.Address = this.Address.Trim();
            data.Port = this.Port.Trim();
            return data;
        }

        public void LoadData(BbsControlData data)
        {
            textBox_name.Text = data.Name;
            textBox_address.Text = data.Address;
            textBox_port.Text = data.Port;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (!PathValidation.IsValidPath(textBox_name.Text))
            {
                this.textBox_name.Focus();
                this.textBox_name.BackColor = Color.Red;
                this.button_save.Enabled = false;
                return;
            }

            var data = this.SaveData();

            var d = Directory.GetCurrentDirectory();
            var p = Path.Combine(d, "BBS");
            Directory.CreateDirectory(p);
            p = Path.Combine(p, data.Name);
            Directory.CreateDirectory(p);
            p = Path.Combine(p, "bbs.xml");

            MMudTerm.SerializeToXmlAndWriteToFile(data, p);
            this.button_save.Enabled = false;
        }

        private void textBox_name_TextChanged(object sender, EventArgs e)
        {
            this.button_save.Enabled = true;
        }
    }

    [Serializable]
    public class BbsControlData
    {
        public BbsControlData()
        {
        }

        public string Port { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
    }



    public class PathValidation
    {
        public static bool IsValidPath(string path)
        {
            // Get invalid path characters (platform-specific)
            char[] invalidPathChars = Path.GetInvalidPathChars();

            // Check if the path contains any invalid characters
            foreach (char c in invalidPathChars)
            {
                if (path.Contains(c))
                {
                    return false; // Invalid character found
                }
            }

            // Additional check for other illegal characters or patterns in the path
            // Note: Checking for ":" is simplistic and Windows-specific; more sophisticated checks might be needed
            if (path.Contains(":") && !path.StartsWith("C:\\") && !path.StartsWith("D:\\") && !path.StartsWith("\\\\"))
            {
                // Example of additional invalid pattern check
                return false;
            }

            // No invalid characters found, path is likely valid
            return true;
        }
    }
}
