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

        delegate void UpdateData(Dictionary<string, TrackedPlayer> data);
        UpdateData DataHandler_Delegate;

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
                this.myScript.Start();
                this.button1.Text = "Stop";
            }
            else
            {
                this.myScript.Stop();
                this.m_connObj.Disconnect();
                this.m_connObj = null;
                this.button1.Text = "Start";
            }
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
            Dictionary<string, TrackedPlayer> NewPlayersData = (e.UserState as Dictionary<string, TrackedPlayer>);
            this.Invoke(DataHandler_Delegate, NewPlayersData);
        }

        private void UpdateDataHandler(Dictionary<string, TrackedPlayer> data)
        {
            foreach (TrackedPlayer p in data.Values)
            {
                DataGridViewRow row = FindRowByPlayerName(p);
                UpdateRowWithPlayerData(row, p);
            }
        }

        private DataGridViewRow FindRowByPlayerName(TrackedPlayer player)
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
                this.dataGridView1.Rows[idx].Cells["fName"].Value = player.FirstName;
                result = this.dataGridView1.Rows[idx];
            }
            return result;
        }

        private void UpdateRowWithPlayerData(DataGridViewRow row, TrackedPlayer p)

        {
            row.Cells["lName"].Value = p.LastName ?? "";
            row.Cells["Alignment"].Value = p.Alignment ?? "";
            row.Cells["Title"].Value = p.Title ?? "";
            row.Cells["Gang"].Value = p.GangName ?? "";
            row.Cells["LevelRange"].Value = p.LevelRange ?? "";
            row.Cells["Level"].Value = p.Level;
            row.Cells["InitExp"].Value = p.InitialExp;
            row.Cells["Exp"].Value = p.Exp;
            row.Cells["Rank"].Value = p.Rank;
            row.Cells["Class"].Value = p.Class != null ? p.Class.Name : "";
            row.Cells["Race"].Value = p.Race != null ? p.Race.Name : "";
            row.Cells["LastExpGain"].Value = p.LastExpGained;
            row.Cells["LastExpRate"].Value = p.LastExpRate;
            row.Cells["TotalExpGain"].Value = p.TotalExpGained;
            row.Cells["TotalExpRate"].Value = p.TotalExpRate;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
