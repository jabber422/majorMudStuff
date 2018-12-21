using MMudTerm;
using MMudTerm.Connection;
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

namespace MummyStreamProcessor
{
    public partial class Form1 : Form
    {
        ConnObj m_connObj = null;
        SessionConnectionInfo mySessionInfo = null;

        Script myMummyScript = null;

        public Form1()
        {
            InitializeComponent();

            this.mySessionInfo = new SessionConnectionInfo();
            this.mySessionInfo.Ip = "127.0.0.1";
            this.mySessionInfo.Port = 12345;
        }

        internal int ConnectToServer()
        {
            int result = 0;
            if (this.m_connObj == null)
            {
                Debug.WriteLine("SessionController - ConnterToServer - ConObj is null, making a new one");
                this.m_connObj = new ConnObj(this.mySessionInfo.IpA, this.mySessionInfo.Port);
                result = 1;
            }

            //connect to the remote
            if (SocketHandler.Connect(this.m_connObj))
            {
                Debug.WriteLine("Connected!");
                this.myMummyScript = new Script(this.m_connObj);             
                this.m_connObj.Disconnected += new EventHandler(ConnHandler_Disconnected);
                result = 2;
            }
            else
            {
                this.m_connObj.mySocket.Close();
                this.m_connObj = null;
                result = 3;
            }

            return result;
        }

        private void ConnHandler_Disconnected(object sender, EventArgs e)
        {
            this.myMummyScript.Stop();
            this.m_connObj = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectToServer();
        }
    }
}
