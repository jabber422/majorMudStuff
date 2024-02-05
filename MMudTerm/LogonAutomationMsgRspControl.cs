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
    public partial class LogonAutomationMsgRspControl : UserControl
    {
        public string Msg { get { return this.textBox_msg_accountname.Text; } set { this.textBox_msg_accountname.Text = value; } }
        public string Rsp { get { return this.textBox_accountname.Text; } set { this.textBox_accountname.Text = value; } }
        public bool IsEnabled { get { return this.checkBox3.Checked; } set { this.checkBox3.Checked = value; } }

        public LogonAutomationMsgRspControl()
        {
            InitializeComponent();
        }

        public LogonRspData SaveData()
        {
            var data = new LogonRspData();
            data.Msg = this.Msg;
            data.Rsp = this.Rsp;
            data.IsEnabled = this.IsEnabled;
            return data;
        }

        public void LoadData(LogonRspData data)
        {
            this.Msg = data.Msg;
            this.Rsp = data.Rsp; 
            this.IsEnabled = data.IsEnabled;
        }

        public Tuple<string, string> GetMessage() {
            if(!this.checkBox3.Checked) { return null; }

            //msg is the regex pattern we will respond to
            string msg = this.textBox_msg_accountname.Text.Trim();
            string rsp = this.textBox_accountname.Text.Trim();

            if(rsp.StartsWith("%") && rsp.EndsWith("%"))
            {
                string env_rsp = rsp.Substring(1, rsp.Length - 1).Remove(rsp.Length - 2);
                //windows env variable
                env_rsp = Environment.GetEnvironmentVariable(env_rsp);

                if(env_rsp == null)
                {
                    throw new Exception("Failed to expand env variable: " + rsp);
                }
                rsp = env_rsp;
            }

            if (msg != string.Empty && rsp != string.Empty) {
                return new Tuple<string, string>(msg, rsp);
            }
            return null;
        }
    }

    [Serializable]
    public class LogonRspData
    {
        public bool IsEnabled { get; set; }
        public string Rsp { get; set; }
        public string Msg { get; set; }
    }
}
