using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TelnetProxyServer.TelnetClient;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using TelnetProxyServer.TelnetProxy;
using System.Threading.Tasks;

namespace TelnetProxyServer
{
    //main control interface for the proxy server
    //controls the listener functionality
    /*scope: wait for a client connection,
     *  auth the local client - not needed yet
     *  send request for <remote address, port> - "Remote Info:"
     *  Connect to remote
     *  Link the local and remote rx/tx streams
     *  store session
     *  fire event with new session info
     */
    public class TelnetProxyServerMain
    {
        TelnetConnectionListener m_listener;
        static Dictionary<int, ITelnetProxySessionControl> m_sessions;

        delegate void NewClientConnectionComplete(int index);

        WaitCallback cb_OnStartNewTelnetProxySession;

        public TelnetProxyServerMain(IPAddress hostIp, int hostPort)
        {
            this.cb_OnStartNewTelnetProxySession = new WaitCallback(OnNewClientStart);

            m_sessions = new Dictionary<int, ITelnetProxySessionControl>();
            this.m_listener = new TelnetConnectionListener(hostIp, hostPort);
            this.m_listener.NewClientConnectEvent += new TelnetConnectionListener.NewClient(m_listener_NewClientConnectEvent);
        }

        /// <summary>
        /// Starts the TcpListner
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            Debug.WriteLine("Starting the proxy server listener", this);
            return this.m_listener.Start();
        }

        /// <summary>
        /// Stops the TcpListener
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            Debug.WriteLine("Stopping the proxy listener", this);
            try
            {
                this.m_listener.Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Stop failed: " + ex.Message + "\r\n" + ex.StackTrace, this);
                return false;
            }
            return true;
        }

        //callback from the tcp listner, rcvs the local connection request from mega
        void m_listener_NewClientConnectEvent(System.Net.Sockets.TcpClient newClient)
        {
            Debug.WriteLine("ENTER m_listener_NewClientConnectEvent");
            //the connection to/from mega
            TelnetSession newSession = new TelnetSession(newClient);
            //wrapper to hold loval and remote connections
            //ITelnetProxySessionControl newProxySession = new TelnetProxySession(newSession);
            //prompt mega for remote connection info
            ThreadPool.QueueUserWorkItem(cb_OnStartNewTelnetProxySession, newSession);
        }

        //Every time there is a new connection do this.
        void OnNewClientStart(object newSessionObject)
        {
            TelnetSession newSession = (TelnetSession)newSessionObject;

            //blocks while the connection to the remote server is started
            NewConnectionScript Script = new NewConnectionScript(); ;
            Task<int> foo = Script.RunLogonScript(newSession, m_sessions);
            //Debug.WriteLine("After Script retval = " + retVal);

            //if (retVal < 0)
            //{
            //    Debug.WriteLine("New Client failed to complete start script succesfully");
            //    newSession.Disconnect();
            //}
            //else
            //{
            //    //connect listener to the watched sockets disconnect event
            //    m_sessions[retVal].DisconnectEvent += newSession_Disconnect_Event;
            //}
        }

        void newSession_Disconnect_Event(object sender, EventArgs e)
        {
            ITelnetProxySessionControl session = (ITelnetProxySessionControl)sender;
            m_sessions.Remove(session.GetId);
            session.Dispose();
        }
    }
}
