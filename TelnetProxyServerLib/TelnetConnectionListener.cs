using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace TelnetProxyServer
{
    public class TelnetConnectionListener
    {
        TcpListener m_listener;

        public delegate void NewClient(TcpClient newClient);
        public event NewClient NewClientConnectEvent;

        AsyncCallback AcceptNewClientMethod;

        public TelnetConnectionListener(IPAddress ip, int port)
        {
            this.m_listener = new TcpListener(ip, port);
            AcceptNewClientMethod = new AsyncCallback(AcceptNewClient);
        }

        /// <summary>
        /// Start the listner socket
        /// </summary>
        /// <returns>t/f</returns>
        public bool Start()
        {
            bool retVal = false;

            if (NewClientConnectEvent == null)
            {
                Debug.WriteLine("There is no one using OnClientConnect event", this.ToString());
                return retVal;
            }

            try
            {
                this.m_listener.Start();
                IAsyncResult ar = this.m_listener.BeginAcceptSocket(AcceptNewClientMethod, null);
                retVal = true;
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("TelnetConnectListener.Start() threw an Exception: " + ex.Message + "\r\n" + ex.StackTrace);
                retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Stop the listener
        /// </summary>
        public void Stop()
        {
            try
            {
                this.m_listener.Stop();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("TelnetConnectListener.Stop() threw an Exception: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void AcceptNewClient(IAsyncResult ar)
        {
            TcpClient newClient = null;
            try{
                newClient = this.m_listener.EndAcceptTcpClient(ar);
                Debug.WriteLine("New Client Connected: " + newClient.Client.RemoteEndPoint.ToString(), this.ToString());
                OnNewClientConnectEvent(newClient);
                this.m_listener.BeginAcceptSocket(AcceptNewClientMethod, null);
            }catch(SocketException ex){
                Debug.WriteLine("AcceptNewClient.Start() threw an Exception: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void OnNewClientConnectEvent(TcpClient client)
        {
            if (this.NewClientConnectEvent == null)
            {
                Debug.WriteLine("NewClientConnectEvent has no listeners", this.ToString());
                return;
            }

            this.NewClientConnectEvent.Invoke(client);
        }
    }
}
