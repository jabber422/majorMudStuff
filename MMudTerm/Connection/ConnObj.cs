using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace MMudTerm.Connection
{
    //simple object to hold send/rcv messages
    public class ConnObj //: ISocketSideHandler, ISessionSideHandler
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
        { get; set; }// { return this.ip; } }

        public int Port
        { get; set; }// { return this.port; } }

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

        internal void BroadcastRcv(byte[] buffer)
        {
            if (Rcvr != null)
            {
                Console.WriteLine("{0} -> Send()", this.Name);
                Rcvr(buffer);
            }
        }

        internal void BroadcastDisconnected()
        {
            if (Disconnected != null)
            {
                Disconnected(this, null);
                //Disconnected = null;
            }
        }

        public void Send(byte[] buffer)
        {
            Console.WriteLine("{0} -> Send()", this.Name);
            SocketHandler.Send(this, buffer);
        }

        internal bool Connect()
        {
            return SocketHandler.Connect(this);
        }

        internal void Disconnect()
        {
            SocketHandler.Disconnect(this);
        }
    }
}
