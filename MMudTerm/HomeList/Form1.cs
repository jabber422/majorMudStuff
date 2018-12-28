using MMudObjects;
using MMudTerm;
using MMudTerm_Protocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeList
{
    public partial class Form1 : Form
    {
        ConnObj m_connObj = null;
        SessionConnectionInfo mySessionInfo = null;

        Script myScript = null;

        delegate void UpdateData(Dictionary<string, Player> data);
        UpdateData DataHandler_Delegate;

        DataTable myData;
        BindingSource myDataSource;

        public Form1()
        {
            InitializeComponent();

            this.mySessionInfo = new SessionConnectionInfo();
            this.mySessionInfo.Ip = "127.0.0.1";
            this.mySessionInfo.Port = 12345;

            //this.myData
            DataHandler_Delegate = new UpdateData(UpdateDataHandler);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text == "Start")
            {
                this.myScript.NewWhoInfo += MyScript_NewWhoInfo;
                this.myScript.Start();
                this.button1.Text = "Stop";
            }
            else
            {
                this.myScript.Stop();
                this.myScript.NewWhoInfo -= MyScript_NewWhoInfo;
                this.m_connObj.Disconnect();
                this.m_connObj = null;
                this.button1.Text = "Start";
            }
        }

        private void MyScript_NewWhoInfo(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(this.button2.Text == "Connect")
            {
                if (this.m_connObj == null)
                {
                    Debug.WriteLine("SessionController - ConnterToServer - ConObj is null, making a new one");
                    this.m_connObj = new ConnObj(this.mySessionInfo.IpA, this.mySessionInfo.Port);
                    this.myScript = new Script(this.m_connObj);
                    this.myScript.m_workerThread.ProgressChanged += scriptWorkerThread_ProgressChanged;
                    this.button1.Enabled = true;
                }

                SocketHandler.Connect(this.m_connObj);
                byte[] rcv = new byte[3];
                int cnt = this.m_connObj.mySocket.Receive(rcv);
                if(cnt == 3)
                {
                    this.myScript.Send("ID,2,42");
                }
                else
                {
                }
                
                this.button2.Text = "Disconnect";
            }
            else
            {
                this.m_connObj.Disconnect();
                this.button2.Text = "Connect";
            }
        }

        //fires when the who data is updated
        private void scriptWorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dictionary<string, Player> NewPlayersData = (e.UserState as Dictionary<string, Player>);
            this.Invoke(DataHandler_Delegate, NewPlayersData);
        }

        private void UpdateDataHandler(Dictionary<string, Player> data)
        {
            foreach (Player p in data.Values)
            {
                DataGridViewRow row = FindRowByPlayerName(p);
                UpdateRowWithPlayerData(row, p);
            }
        }

        private DataGridViewRow FindRowByPlayerName(Player player)
        {
            DataGridViewRow result = null;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if(row.Cells["fName"].Value == null) continue;
                if(row.Cells["fName"].Value.ToString() != player.FirstName) continue;
                return row;
            }

            if(result == null)
            {
                int idx = this.dataGridView1.Rows.Add();
                result = this.dataGridView1.Rows[idx];
            }
            return result;
        }

        private void UpdateRowWithPlayerData(DataGridViewRow row, Player p)
        {
            row.Cells["lName"].Value = p.LastName != null ? p.LastName : "";
            row.Cells["Alignment"].Value = p.Alignment != null ? p.LastName : "";
            row.Cells["Title"].Value = p.Title != null ? p.LastName : "";
            row.Cells["Gang"].Value = p.GangName != null ? p.LastName : "";
        }
    }
}
