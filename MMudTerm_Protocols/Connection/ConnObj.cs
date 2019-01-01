using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using MMudObjects;

namespace MMudTerm_Protocols
{
    //simple object to hold send/rcv messages
    public class ConnObj
    {
        const int BUFFSIZE = 2048;
        byte[] buffer;
        protected Socket soc;
        IPAddress ip;
        int port;

        public string Name { get; set; }

        public event EventHandler Disconnected;
        public event RcvMsgCallback Rcvr;
        
        public byte[] Buffer
        {get{return this.buffer;}}

        public Socket mySocket
        { get { return this.soc; } }

        public IPAddress Ip
        { get; set; }

        public int Port
        { get; set; }

        public bool Connected
        { 
            get { return this.soc.Connected; }
        }

        public ConnObj(Socket s)
        {
            this.soc = s;
            soc.Blocking = false;
            buffer = new byte[BUFFSIZE];
            SocketHandler.BeginReceive(this);
        }

        public ConnObj(IPAddress ip, int port)
        {
            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soc.Blocking = false;
            buffer = new byte[BUFFSIZE];
            this.Ip = ip;
            this.Port = port;
        }

        //incoming raw buffer from the server socket
        virtual public void BroadcastRcv(byte[] buffer)
        {
            if (Rcvr != null)
            {
//                Debug.WriteLine("{0} -> Send()", this.Name);
                Log.Tag("ConnObj", "Rcvr -> {0}", ASCIIEncoding.ASCII.GetString(buffer));
                Rcvr(buffer);
            }
        }

        virtual public void BroadcastDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, null);
            }
        }

        //outgoing raw buffer to the server socket
        public void Send(byte[] buffer)
        {
            Log.Tag("ConnObj", "Send -> {0}", ASCIIEncoding.ASCII.GetString(buffer));
            SocketHandler.Send(this, buffer);
        }

        public bool Connect()
        {
            return SocketHandler.Connect(this);
        }

        public void Disconnect()
        {
            SocketHandler.Disconnect(this);
        }
    }
}
