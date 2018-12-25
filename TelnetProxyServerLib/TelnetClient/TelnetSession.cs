using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;
using TelnetProxyServer;

namespace TelnetProxyServer.TelnetClient
{
    //This should encapsulate a telnet session between the proxy server and the local client or a remote server
    public class TelnetSession : ITelnetSessionControl
    {
        private TcpClient TcpClient { get { return this.m_TcpClient; } set { this.m_TcpClient = value; } }
        AsyncCallback EndClientReadCallback;
        event EventHandler<DataRcvEvent> m_Receive_Event;
        event EventHandler m_Disconnect_Event;

        public string Name { get { return this.m_name; } set { this.m_name = value; } }
        //public IAsyncResult CurrentAsyncHandler { get; private set; }
        //public MyBuffer TcpClientReadBuffer { get; private set; }
        string m_name;
        TcpClient m_TcpClient;
        string m_remoteIp;
        int m_remotePort;
        Guid m_Id;

        #region ITelnetSessionControl
        public string IpAddress { get { return m_remoteIp; } }
        public int Port { get { return m_remotePort; } }
        public Guid Id { get { return m_Id; } }
        public bool IsConnected { get { return (this.m_TcpClient == null) ? false : this.m_TcpClient.Connected; } }

        public event EventHandler<DataRcvEvent> Receive_Event
        {
            add { this.m_Receive_Event += value; }
            remove { this.m_Receive_Event -= value; }
        }

        public event EventHandler Disconnect_Event
        {
            add { this.m_Disconnect_Event += value; }
            remove { this.m_Disconnect_Event -= value; }
        }

        /// <summary>
        /// writes a buffer to the tcp client stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SendToRemote(byte[] buffer)
        {
            if (this.TcpClient == null)
            {
                Debug.WriteLine("Attempt to write to a null TcpClient, bad juju", this);
                return -1;
            }

            if (!this.TcpClient.Connected)
            {
                Debug.WriteLine("Attempt to write to disconnected TcpClient, bad kuku", this);
                return -1;
            }

            try
            {
                this.TcpClient.GetStream().Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TelnetSession.Send threw ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
                return -1;
            }
            return buffer.Length;
        }

        /// <summary>
        /// fakes like the telnetsession recieved something from the remote client
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SendToLocal(byte[] buffer)
        {
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(OnDataRcv), buffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TelnetSession.Send threw ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
                return -1;
            }
            return buffer.Length;
        }

        public void SetTcpClient(TcpClient client)
        {
            this.TcpClient = client;
            IPEndPoint ip = (IPEndPoint)this.TcpClient.Client.RemoteEndPoint;
            this.m_remoteIp = ip.Address.ToString();
            this.m_remotePort = ip.Port;
        }

        public bool Connect()
        {
            if (this.TcpClient == null)
            {
                Debug.WriteLine("Calling connect on a null TcpClient... bad juju", this);
                return false;
            }
            try
            {
                if (!this.TcpClient.Connected)
                {
                    this.TcpClient.Connect(m_remoteIp, m_remotePort);
                }
                else
                {
                    Debug.WriteLine("Connect called on a connected tcpclient", this);
                }
                BeginRead(new byte[8192]);
                return this.TcpClient.Connected;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TelnetSession.Connect threw ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
            }
            return false;
        }

        public bool Disconnect()
        {
            if (!this.TcpClient.Connected)
            {
                Debug.WriteLine("Cannot disconnect, we are not connected", this);
                return true;
            }

            this.TcpClient.GetStream().Close();
            this.TcpClient.Close();
            OnDisconnect(null);
            return true;
        }

        public void BeginRead()
        {
            this.BeginRead(new byte[8192]);
        }
        #endregion

        #region ctor
        public TelnetSession(TcpClient client)
            : this()
        {
            //this.m_Id = Guid.NewGuid();
            //this.EndClientReadCallback = new AsyncCallback(EndRead);
            //this.TcpClientReadBuffer = new MyBuffer();
            this.SetTcpClient(client);
        }

        public TelnetSession(string remoteIp, int remotePort)
            : this()
        {
            //this.m_Id = Guid.NewGuid();
            //this.EndClientReadCallback = new AsyncCallback(EndRead);
            //this.TcpClientReadBuffer = new MyBuffer();
            try
            {
                this.SetTcpClient(new TcpClient(remoteIp, remotePort));
            }
            catch (Exception ex)
            {

            }
        }

        public TelnetSession()
        {
            this.m_Id = Guid.NewGuid();
            this.EndClientReadCallback = new AsyncCallback(EndRead);
            //this.TcpClientReadBuffer = new MyBuffer();
        }
        #endregion

        #region privates
        private void BeginRead(byte[] buffer)
        {
            try
            {
                this.TcpClient.GetStream().BeginRead(buffer, 0, buffer.Length, this.EndClientReadCallback, buffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TelnetSession.BeginRead threw ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
            }
        }

        private void EndRead(IAsyncResult ar)
        {
            try
            {
                byte[] buffer = (ar.AsyncState as byte[]);
                int size = this.TcpClient.GetStream().EndRead(ar);
                if (size <= 0)
                {
                    //SocketError?
                    Debug.WriteLine("read size was <= 0", this);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(OnDisconnect));
                    return;
                }

                lock (buffer)
                {
                    byte[] rcvdBytes = new byte[size];
                    Buffer.BlockCopy(buffer, 0, rcvdBytes, 0, size);
                    //this.TcpClientReadBuffer.Size = size;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(OnDataRcv), rcvdBytes);
                }
                BeginRead(buffer);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.WriteLine("TelnetSession.EndRead threw objDisposed ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TelnetSession.EndRead threw ex: " + ex.Message + "\r\n" + ex.StackTrace, this);
            }

        }

        private void OnDataRcv(object state)
        {
            if (m_Receive_Event == null)
            {
                Debug.WriteLine("No rcv listeners on session reader", this);
                return;
            }

            DataRcvEvent ev = new DataRcvEvent((byte[])state);
            this.m_Receive_Event.Invoke(this, ev);
        }

        private void OnDisconnect(object state)
        {
            lock (this.m_TcpClient)
            {
                if (m_Disconnect_Event == null)
                {
                    Debug.WriteLine("No disconnect listeners on session reader", this);
                    return;
                }

                this.m_Disconnect_Event.Invoke(this, new SessionsDisconnectEvent(""));
            }
        }
        #endregion
    }

  
}
