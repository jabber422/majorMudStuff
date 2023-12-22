using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using TelnetProxyServer.TelnetClient;
using TelnetProxyServer.TelnetProxy;

namespace TelnetProxyServer
{
    internal class IdSessionState : StartScriptState
    {
        private int id;
        
        const string csvRegex_new = "CON,(\\S+),(\\S+)";


        public IdSessionState(StartScriptState state, int id) : base(state)
        {
            this.id = id;
            this.Write("CON?");
        }

        public IdSessionState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions) : base(newSession, sessions)
        {
        }

        public override StartScriptState Handle(string a)
        {
            int res = 0;
            Match m = Regex.Match(a, csvRegex_new);
            if (m == null && !m.Success)
                res = BAD_ID_REGEX;

            string ip = m.Groups[1].Value;
            int port = -1;

            if (!int.TryParse(m.Groups[2].Value, out port))
            {
                this.Write("Bad Port!\r\n");
                return new IdState(this);
            }

            try
            {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(ip);
                if (entry.AddressList.Length == 0)
                {
                    Debug.WriteLine("Failed to resolve: " + ip, "StartScript");
                    res = BAD_ID;
                }
                ip = entry.AddressList[0].ToString();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to parse ip or port from: " + ip + " " + port, "StartScript");
                return new IdState(this);
            }

            ITelnetSessionControl remoteSession = new TelnetSession(ip, port);
            remoteSession.Name = "Remote";
            if (!remoteSession.Connect())
            {
                Debug.WriteLine("Failed to connected to Remote Server");
                this.Write("Failed to connected to Remote Server");
                return new IdSessionState(this, this.id);
            }

            ITelnetProxySessionControl newProxySession = new TelnetProxySession(this._newSession, id);
            newProxySession.AddSession(ETelnetProxySession.Remote, remoteSession);
            newProxySession.AddSession(ETelnetProxySession.Client, this._newSession);
            remoteSession.BeginRead();
            this._newSession.BeginRead();

            this._sessions.Add(id, newProxySession);

            return new WorkingState(this);
        }

        public override StartScriptState handle(ref byte[] buffer, ref int offset, ref int count)
        {
            if (buffer[offset] == 255 || buffer[offset] == (byte)'\r')
            {
                Debug.WriteLine("In new Session State buf got telnet gunk!");
                var next_state = new TelnetState(this).handle(ref buffer, ref offset, ref count);
                
                while (next_state is TelnetState)
                {
                    if (count == 0) { return this; }
                    Debug.WriteLine("Statying in Telnet State");
                    next_state = new TelnetState(this).handle(ref buffer, ref offset, ref count);
                }
            }

            if (count == 0)
            {
                return this;
            }
            //We got a buffer, we want to process from 0 to offset + count
            string line = ASCIIEncoding.ASCII.GetString(buffer, offset, buffer.Length - offset).Trim();

            int type, id, result = 0;
            //we got a response to "ID?"
            var new_state = this.Handle(line);
            if (new_state is IdState)
            {
                //if we get IdState back that means we don't like the buffer, clear it out and start over
                buffer = new byte[4096]; offset = 0; count = 0;
                Debug.WriteLine("Ignored: " + line);
            }
            else
            {
                //we took this line, update the buffer
                byte[] new_buffer = new byte[4096];
                Buffer.BlockCopy(buffer, count, new_buffer, 0, buffer.Length - count);
                buffer = new_buffer;
                offset = 0;
                count = 0;
                Debug.WriteLine("Took: " + line);
            }
            return new_state;
        }
    }

    internal class IdListenerState : StartScriptState
    {

        private int id;

        public IdListenerState(StartScriptState state, int id) : base(state)
        {
            //this.id = id;
            //if (!this._sessions.ContainsKey(id))
            //{
            //    this.Write("Sessions does not Exist! :" + id + "\r\n");
            //    return new IdState(this);
            //}
            //else
            //{
            //    this._sessions[id].AddSession(ETelnetProxySession.Tap, this._newSession);
            //    return new WorkingState(this);
            //}
        }

        public IdListenerState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions) : base(newSession, sessions)
        {
        }

        public override StartScriptState Handle(string a)
        {
            throw new System.NotImplementedException();
        }

        public override StartScriptState handle(ref byte[] buffer, ref int offset, ref int count)
        {
            throw new System.NotImplementedException();
        }
    }
}