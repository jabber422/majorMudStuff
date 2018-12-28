using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetProxyServer.TelnetClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net.Sockets;


namespace TelnetProxyServer.TelnetProxy
{
    //contains 2+ TelnetSession one from the local mega mud, one for the remote bbs, third would be the tapping session
    //provides methods to hook into the IO streams
    internal class TelnetProxySession : ITelnetProxySessionControl
    {
        ITelnetSessionControl client;
        ITelnetSessionControl remote;
        ITelnetSessionControl tap;
        //Dictionary<int, TelnetSession> Sessions;
        public ITelnetSessionControl ClientSession { get { return this.client; } }
        public ITelnetSessionControl RemoteSession { get { return this.remote; } }
        public ITelnetSessionControl TapSession { get { return this.tap; } }

        //Event that the object consume can use to read the streams and handle disconnect
        public event EventHandler DisconnectEvent;

        EventHandler<DataRcvEvent> RcvFromClientSession_EventHandler;
        EventHandler<DataRcvEvent> RcvFromRemoteSession_EventHandler;
        EventHandler<DataRcvEvent> RcvFromTapSession_EventHandler;

        EventHandler ClientSessionDisconnect_EventHandler;
        EventHandler RemoteSessionDisconnect_EventHandler;
        EventHandler TapSessionDisconnect_EventHandler;
        
        bool m_TapBlockRcvFromRemote = false;
        bool m_TapBlockRcvFromClient = false;

        int m_id = -1;
        public int GetId
        {
            get
            {
                return this.m_id;
            }
        }
        
        /// <summary>
        /// Wraps the local and remote client
        /// </summary>
        /// <param name="ClientSession">the client that just connected, will become the client session</param>
        internal TelnetProxySession(ITelnetSessionControl ClientSession, int Id)
        {
            this.m_id = Id;

            RcvFromClientSession_EventHandler = new EventHandler<DataRcvEvent>(RcvFromClientSession);
            RcvFromRemoteSession_EventHandler = new EventHandler<DataRcvEvent>(RcvFromRemoteSession);
            RcvFromTapSession_EventHandler = new EventHandler<DataRcvEvent>(RcvFromTapSession);

            ClientSessionDisconnect_EventHandler = new EventHandler(ClientSessionDisconnect);
            RemoteSessionDisconnect_EventHandler = new EventHandler(RemoteSessionDisconnect);
            TapSessionDisconnect_EventHandler = new EventHandler(TapSessionDisconnect);

            client = ClientSession;
            client.Receive_Event += RcvFromClientSession_EventHandler;
            client.Disconnect_Event += ClientSessionDisconnect_EventHandler;
            client.Name = "Client";
        }

        #region event handlers from the telnet sessions
        /// <summary>
        /// The local client has disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">nothing</param>
        void ClientSessionDisconnect(object sender, EventArgs e)
        {
            Trace.WriteLine("TPS, Id=" + this.GetId + " Client Disconnect event fired", this.ToString());
            if (this.DisconnectEvent != null)
            {
                this.DisconnectEvent.Invoke(this, new SessionsDisconnectEvent("Client"));
            }

            RemoveSession(ETelnetProxySession.Client);
        }
        
        void RemoteSessionDisconnect(object sender, EventArgs e)
        {
            Trace.WriteLine("TPS, Id=" + this.GetId + " Remote Client Disconnect event fired", this.ToString());
            if (this.DisconnectEvent != null)
            {
                this.DisconnectEvent.Invoke(this, new SessionsDisconnectEvent("Remote"));
            }
            RemoveSession(ETelnetProxySession.Remote);

        }

        void TapSessionDisconnect(object sender, EventArgs e)
        {
            Trace.WriteLine("TPS, Id=" + this.GetId + " Tap Session Disconnect event fired", this.ToString());
            if (this.DisconnectEvent != null)
            {
                this.DisconnectEvent.Invoke(this, e);
            }
            RemoveSession(ETelnetProxySession.Tap);
        }
        #endregion

        #region Receive from telnet session handlers
        //Got something from the local send it to the remote and listen(ers)
        public void RcvFromClientSession(object sender, DataRcvEvent e)
        {
            if (this.TapSession != null && this.TapSession.IsConnected)
            {
                this.TapSession.SendToRemote(e.DataBuffer);
            }

            if (!this.m_TapBlockRcvFromClient)
            {
                if (this.RemoteSession != null && this.RemoteSession.IsConnected)
                {
                    this.RemoteSession.SendToRemote(e.DataBuffer);
                }
            }
        }

        /// <summary>
        /// Got something from the Remote source, Send it to listen(ers)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RcvFromRemoteSession(object sender, DataRcvEvent e)
        {
            if (this.TapSession != null && this.TapSession.IsConnected)
            {
                this.TapSession.SendToRemote(e.DataBuffer);
            }

            if (!this.m_TapBlockRcvFromRemote)
            {
                if (this.ClientSession != null && this.ClientSession.IsConnected)
                {
                    this.ClientSession.SendToRemote(e.DataBuffer);
                }
            }
        }

        public void RcvFromTapSession(object sender, DataRcvEvent e)
        {
            if (this.RemoteSession.IsConnected)
            {
                this.RemoteSession.SendToRemote(e.DataBuffer);
            }
        }
        #endregion

        #region ITelnetProxySessionControl
        public void DisconnectSession(ETelnetProxySession eClientType)
        {
            switch (eClientType)
            {
                case ETelnetProxySession.Client:
                    this.ClientSession.Receive_Event -= RcvFromClientSession_EventHandler;
                    this.ClientSession.Disconnect_Event -= ClientSessionDisconnect_EventHandler;
                    Disconnect(this.ClientSession);
                    break;
                case ETelnetProxySession.Remote:
                    this.ClientSession.Receive_Event -= RcvFromRemoteSession_EventHandler;
                    this.ClientSession.Disconnect_Event -= RemoteSessionDisconnect_EventHandler;
                    Disconnect(this.RemoteSession);
                    break;
                case ETelnetProxySession.Tap:
                    this.ClientSession.Receive_Event -= RcvFromTapSession_EventHandler;
                    this.ClientSession.Disconnect_Event -= TapSessionDisconnect_EventHandler;
                    Disconnect(this.TapSession);
                    break;
                default:
                    throw new Exception("TPS - SendTo, ETelnetProxySession out of range");
            }
        }

        private void Disconnect(ITelnetSessionControl telnetSession)
        {
            Trace.WriteLine("TPS, Id=" + this.GetId + " Disconnect Session: " + telnetSession.Name);
            try
            {
                if (telnetSession.IsConnected)
                {
                    Trace.WriteLine("TPS, Id=" + this.GetId + " Disconnect Session - client was connected still");
                    telnetSession.Disconnect();
                }
                else
                {
                    Trace.WriteLine("TPS, Id=" + this.GetId + " Disconnect Session - client was not connected");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("TPS, Id=" + this.GetId + " DisconnectRemote Session with ex: " + ex.Message + "\r\n" + ex.StackTrace, this.ToString());
                return;
            }
        }

        /// <summary>
        /// remove the remote or tap session from the proxy session
        /// </summary>
        /// <param name="eTelnetProxySession"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public void RemoveSession(ETelnetProxySession eTelnetProxySession)
        {
            switch (eTelnetProxySession)
            {
                case ETelnetProxySession.Client:
                    if (this.client == null) return;
                    DisconnectSession(eTelnetProxySession);
                    this.client = null;
                    break;
                case ETelnetProxySession.Remote:
                    if (this.remote == null) return;
                    DisconnectSession(eTelnetProxySession);
                    this.remote = null;
                    break;
                case ETelnetProxySession.Tap:
                    if (this.tap == null) return;
                    DisconnectSession(eTelnetProxySession);
                    this.tap = null;
                    break;
            }
        }

        /// <summary>
        /// Adds the remote or tap session to the proxy session
        /// </summary>
        /// <param name="eTelnetProxySession"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public void AddSession(ETelnetProxySession eTelnetProxySession, ITelnetSessionControl session)
        {
            switch (eTelnetProxySession)
            {
                case ETelnetProxySession.Remote:
                    this.remote = session;
                    this.RemoteSession.BeginRead();
                    this.RemoteSession.Receive_Event += RcvFromRemoteSession_EventHandler;
                    this.RemoteSession.Disconnect_Event += RemoteSessionDisconnect_EventHandler;
                    break;
                case ETelnetProxySession.Tap:
                    this.tap = session;
                    this.TapSession.BeginRead();
                    this.TapSession.Receive_Event += RcvFromTapSession_EventHandler;
                    this.TapSession.Disconnect_Event += TapSessionDisconnect_EventHandler;
                    break;
            }
        }

        /// <summary>
        /// Sends a buffer to the locally connected client
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public int SendTo(ETelnetProxySession eClientType, string str)
        {
            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(str);
            int size = -1;
            switch(eClientType){
                case ETelnetProxySession.Client:
                    size = SendTo(this.ClientSession, buffer);
                    break;
                case ETelnetProxySession.Remote:
                    size = SendTo(this.RemoteSession, buffer);
                    break;
                case ETelnetProxySession.Tap:
                    size = SendTo(this.TapSession, buffer);
                    break;
                default:
                    throw new Exception("TPS - SendTo, ETelnetProxySession out of range");
            }
            
            if (size == buffer.Length)
                return size;

            Trace.WriteLine("Failed to write a buffer to the " + eClientType.ToString() + " client", this.ToString());
            return size;
        }

        private int SendTo(ITelnetSessionControl telnetSession, byte[] buffer)
        {
            if (!telnetSession.IsConnected) return -1;
            return telnetSession.SendToRemote(buffer);
        }
        #endregion

        public void Dispose()
        {
            if (this.client != null && this.client.IsConnected) this.client.Disconnect();
            if (this.remote != null && this.remote.IsConnected) this.remote.Disconnect();
            if (this.tap != null && this.tap.IsConnected) this.tap.Disconnect();

            client = null;
            remote = null;
            tap = null;
         }
    }

    

   
}
