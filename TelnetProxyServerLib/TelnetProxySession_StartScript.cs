using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TelnetProxyServer.TelnetClient;
using TelnetProxyServer.TelnetProxy;

namespace TelnetProxyServer
{
    //abstract class StartScriptState
    //{
    //    abstract public void handle(string a);
    //}

    //class IdState : StartScriptState
    //{
    //    public override void handle(string a)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //class ConState: StartScriptState { }

    //class WorkingState : StartScriptState { }

    class TelnetProxySession_StartScript
    {
        StartScriptState _state = null;
        byte[] _buffer = null;

        
        object m_lock = new object();
        Queue<string> rcvd = new Queue<string>();

        bool timeout = false;
        ManualResetEvent mre = new ManualResetEvent(false);

        const int BAD_ID = -1;
        const int BAD_ID_REGEX = -2;
        const int ID_NO_SESSION_FOUND = -3;
        const int ID_BAD_PORT_CAST_TO_INT = -4;
        const int BAD_ID_NON_INT = -5;
        const int BAD_REMOTE_CONNECTION = -6;
        const int BAD_ID_TYPE = -7;
        
        const int ID_NEWSESSION = 1;
        const int ID_LISTENER = 2;

        const string DBG_CAT = "StartScript";

        const string csvRegex_id = "ID,(\\d),(\\d+)";
        const string csvRegex_listener = "CON,(\\d)";
        const string csvRegex_new = "CON,(\\S+),(\\S+)";

        EventHandler<DataRcvEvent> ClientRcvdDataEventHandler;

        public TelnetProxySession_StartScript()
        {
            
            ClientRcvdDataEventHandler = new EventHandler<DataRcvEvent>(newProxySession_ClientDataReceivedListener_Event);
        }

        //when someone connects to the listener this will send Action?
        // possible Action
        // CON - Format would be CON,<ip|hostname>,port
        // LISTEN - Format would be LISTEN

        /// <summary>
        /// Prompts the local client for remote info and connects the streams
        /// </summary>
        /// <param name="newTelnetSession"></param>
        /// <returns>int -1 fail, 0 means a lister succesfully attached to a session, > 0 is a new session id</returns>
        public int Start(ITelnetSessionControl newTelnetSession, Dictionary<int, ITelnetProxySessionControl> sessions)
        {
            Debug.WriteLine("Starting New connection script");
            int type, id, result = 0;
            lock(m_lock) {
                
                //connect to event when the new session talks to us
                newTelnetSession.Receive_Event += ClientRcvdDataEventHandler;
                newTelnetSession.BeginRead();

                //ID,0,<num> - Listen to <num>
                //ID,1,<num> - Create new session called <num>

                //start the sequence
                Match m = BlockUntilAnswered(newTelnetSession, "ID?", csvRegex_id, 5, 5);
                if (m == null  || !m.Success)
                    return BAD_ID_REGEX;

                if (!int.TryParse(m.Groups[1].Value, out type))
                {
                    return BAD_ID_NON_INT;
                }

                if (!int.TryParse(m.Groups[2].Value, out id))
                {
                    return BAD_ID_NON_INT;
                }

                string s = "";
                int res = id;
                if (type == ID_LISTENER)
                {
                    newTelnetSession.Name = "Tap";
                    if (!sessions.ContainsKey(id))
                    {
                        s = "Invalid ID, ID not found: " + id;
                        res = -1;
                    }else{
                        ITelnetProxySessionControl proxySession = sessions[id];
                        proxySession.AddSession(ETelnetProxySession.Tap, newTelnetSession);
                        s = "Tap added to session: " + id;
                    }

                }
                else if (type == ID_NEWSESSION)
                {
                    newTelnetSession.Name = "Client";
                    if (sessions.ContainsKey(id))
                    {
                        s = "Session ID already active: " + id;
                    }
                    else
                    {
                        ITelnetProxySessionControl newProxySession = new TelnetProxySession(newTelnetSession, id);
                        int res2 = CreateNewSessionScript(newProxySession);

                        if(res2 != 1) {
                            s = "Failed to create new remote Session: " + res;
                            res = res2;
                        }else{
                            sessions.Add(id, newProxySession);
                            s = "New Session Created: " + id;
                        }
                    }
                }
                else
                {
                    s = "Invalid Type -> " + type;
                    res = BAD_ID_TYPE;
                }

                Debug.WriteLine("Start - New Telnet session - " + s);
                newTelnetSession.SendToRemote(Encoding.ASCII.GetBytes(s));

                if (res < 0)
                {
                    newTelnetSession.Disconnect();
                }
                else
                {
                    newTelnetSession.Receive_Event -= ClientRcvdDataEventHandler;
                }

                result = res;
            }
            return result;
        }

        private Match BlockUntilAnswered(ITelnetProxySessionControl newProxySession, ETelnetProxySession eTelnetProxySession,
            string p1, string csvRegex_new, int p2, int p3)
        {
            switch (eTelnetProxySession)
            {
                case ETelnetProxySession.Client:
                    return BlockUntilAnswered(newProxySession.ClientSession, p1, csvRegex_new, p2, p3);
                case ETelnetProxySession.Remote:
                    return BlockUntilAnswered(newProxySession.RemoteSession, p1, csvRegex_new, p2, p3);
                case ETelnetProxySession.Tap:
                    return BlockUntilAnswered(newProxySession.TapSession, p1, csvRegex_new, p2, p3);
                default:
                    Debug.WriteLine("Bad switch Block Until Answered");
                    break;
            }
            return Match.Empty;
        }

        private Match BlockUntilAnswered(ITelnetSessionControl session, string p, string csvRegex_id, int pollSec, int pollAttempt)

        {
            Match m = Match.Empty;
            int cnt = 0;
            while (cnt < pollAttempt)
            {
                //when something connects to the listner it sends an ID
                //if mega is connection, send a unique # > 0, this is the lookup for that session
                //if you want to listen to what the remote server is sending, send the id of the session
                session.SendToRemote(Encoding.ASCII.GetBytes(p));

                //process ID response
                m = PendForMatch(csvRegex_id);

                if (m.Success) break;
                
                Thread.Sleep(pollSec * 1000);
                cnt++;
            }

            return m;
        }

        //private static int AttachListenerToSession(int id, ITelnetProxySessionControl newSession, ITelnetProxySessionControl existingSession)
        //{
        //    int result = -1;

        //    if (!existingSession.AttachListener(ETelnetProxySession.Remote, newSession))
        //    {
        //        Debug.WriteLine("Failed to attach listner to Session " + result, DBG_CAT);
        //    }

        //    if (!newSession.AttachListener(ETelnetProxySession.Client, existingSession))
        //    {
        //        Debug.WriteLine("Failed to attach writer to Session " + result, DBG_CAT);
        //    }

        //    return 1;
        //}

        private int CreateNewSessionScript(ITelnetProxySessionControl newProxySession)
        {
            Match m = BlockUntilAnswered(newProxySession, ETelnetProxySession.Client, "CON?", csvRegex_new, 5, 5);
            if (m == null  && !m.Success)
                    return BAD_ID_REGEX;

            string ip = m.Groups[1].Value;
            int port = -1;

            if(!int.TryParse(m.Groups[2].Value, out port)){
                return ID_BAD_PORT_CAST_TO_INT;
            }

            try
            {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(ip);
                if (entry.AddressList.Length == 0)
                {
                    Debug.WriteLine("Failed to resolve: " + ip, "StartScript");
                    return BAD_ID;
                }
                ip = entry.AddressList[0].ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to parse ip or port from: " + ip + " " + port, "StartScript");
                return BAD_ID;
            }

            ITelnetSessionControl remoteSession = new TelnetSession(ip, port);
            remoteSession.Name = "Remote";
            if (!remoteSession.Connect())
            {
                Debug.WriteLine("Failed to connected to Remote Server");
                return BAD_REMOTE_CONNECTION;
            }

            newProxySession.AddSession(ETelnetProxySession.Remote, remoteSession);
            remoteSession.BeginRead();
            return 1;
        }

        private Match PendForMatch(string csvRegex)
        {
            timeout = false;
            Timer t = new Timer(new TimerCallback(OnTimerExpired), null, 10*1000, Timeout.Infinite);

            Match m = Match.Empty;

            string r = "";
            while (!timeout)
            {
                mre.WaitOne(100);
                while (rcvd.Count > 0)
                {
                    r += rcvd.Dequeue();
                    Debug.WriteLine("rcvd: " + r, DBG_CAT);
                    m = Regex.Match(r, csvRegex);
                    if (m.Success)
                        return m;
                }
                mre.Reset();
            }

            return m;
        }

        private void OnTimerExpired(object o)
        {
            timeout = true;
            mre.Set();
        }

        void newProxySession_ClientDataReceivedListener_Event(object sender, DataRcvEvent e)
        {
            string s = ASCIIEncoding.ASCII.GetString(e.DataBuffer);
            rcvd.Enqueue(s);
            mre.Set();
        }
    }
}
