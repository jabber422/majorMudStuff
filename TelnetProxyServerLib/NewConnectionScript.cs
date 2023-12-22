using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelnetProxyServer.TelnetClient;
using TelnetProxyServer.TelnetProxy;

namespace TelnetProxyServer
{
    public abstract class StartScriptState
    {
        protected ITelnetSessionControl _newSession;
        protected Dictionary<int, ITelnetProxySessionControl> _sessions;

        internal const int BAD_ID = -1;
        internal const int BAD_ID_REGEX = -2;
        internal const int ID_NO_SESSION_FOUND = -3;
        internal const int ID_BAD_PORT_CAST_TO_INT = -4;
        internal const int BAD_ID_NON_INT = -5;
        internal const int BAD_REMOTE_CONNECTION = -6;
        internal const int BAD_ID_TYPE = -7;

        internal const int ID_NEWSESSION = 1;
        internal const int ID_LISTENER = 2;

        protected int IAC_253_3 = 0;
        protected int IAC_253_1 = 0;
        protected int IAC_253_0 = 0;
        protected int IAC_251_0 = 0;

        public StartScriptState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions)
        {
            this._newSession = newSession;
            this._sessions = sessions;
        }

        public StartScriptState(StartScriptState state)
        {
            this._newSession = state._newSession;
            this._sessions = state._sessions;
            IAC_253_3 = state.IAC_253_3;
            IAC_253_1 = state.IAC_253_1;
            IAC_253_0 = state.IAC_253_0;
            IAC_251_0 = state.IAC_251_0;
        }

        protected void Write(string a)
        {
            using (var ns = new NetworkStream(this._newSession.GetTcpClient().Client))
            using (var writer = new StreamWriter(ns, Encoding.ASCII))
            {
                writer.Write(a);
            }
        }

        protected void Write(byte[] buffer)
        {
            using (var ns = new NetworkStream(this._newSession.GetTcpClient().Client))
            {
                ns.Write(buffer, 0, buffer.Length);
            }
        }

        virtual public StartScriptState Handle(string a) { return null; }
        abstract public StartScriptState handle(ref byte[] buffer, ref int offset, ref int count);
    }

    public class TelnetState : StartScriptState
    {
        int cnt = 0;

        public TelnetState(StartScriptState state) : base(state)
        {
        }

        public TelnetState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions) : base(newSession, sessions)
        {
            byte[] foo = new byte[] {255,
                251,
                3,
                255,
                251,
                1,
                255,
                251,
                0,
                255,
                253,
                0 };
            this.Write(foo);
        }

        public override StartScriptState handle(ref byte[] buffer, ref int offset, ref int count)
        {
            //if this is 0, then bufer should be 0's, no-op
            if (count == 0) {
                if (this.IAC_253_3 > 1 && this.IAC_251_0 > 0)
                {
                    return new IdState(this, true);
                }
                else
                {
                    return new IdState(this, false);
                }
                return this;
            }

            //clear out the random carraige returns that mega sends
            if (buffer[0] == 13)
            {
                byte[] new_buffer = new byte[4096];
                Buffer.BlockCopy(buffer, 1, new_buffer, 0, buffer.Length-1);
                buffer = new_buffer;
                count -= 1;
                Debug.WriteLine("CR");
                return this.handle(ref buffer, ref offset, ref count);
            }else if (buffer[0] == 255)
            {
                //process incoming IAC commands, this is not to spec 
                byte[] telnet_iac = new byte[3];

                Buffer.BlockCopy(buffer, offset, telnet_iac, 0, 3);
                string s = "";
                s += telnet_iac[0] + " ";
                s += telnet_iac[1] + " ";
                s += telnet_iac[2];

                //If we get here, eat three bytes
                byte[] new_buffer = new byte[4096];
                Buffer.BlockCopy(buffer, offset + 3, new_buffer, 0, buffer.Length - 3);
                buffer = new_buffer;
                count -= 3;

                Debug.WriteLine(s);
                if (s == "255 253 3" )
                {
                    byte[] foo = new byte[] { 255, 251, 3 };
                    if (this.IAC_253_3 == 0)
                    {
                        this.Write(foo);
                    }
                    this.IAC_253_3++;
                }
                else if (s == "255 253 1")
                { //IAC DO Echo
                    //"255 251 1" IAC WILL Echo
                    byte[] foo = new byte[] { 255, 251, 1 };
                    //this.Write(foo);
                    this.IAC_253_1++;
                }
                else if (s == "255 253 0") //IAC DO Binary
                {
                    //255 251 0  IAC WILL Binary
                    byte[] foo = new byte[] { 255, 251, 0 };
                    //this.Write(foo);
                    this.IAC_253_0++;
                }
                else if (s == "255 251 0")
                {
                    byte[] foo = new byte[] { 255, 253, 0, };
                    //this.Write(foo);
                    this.IAC_251_0++;
                }
                else
                {
                }
                
                return this.handle(ref buffer, ref offset, ref count);
            }

            if (this.IAC_253_3 > 0 && this.IAC_251_0 > 0)
            {
                return new IdState(this, true);
            }
            return this;
        }
    }

    public class IdState : StartScriptState
    {
        const string csvRegex_id = "ID,(\\d),(\\d+)";

        public IdState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions) : base(newSession, sessions)
        {
        }

        public IdState(StartScriptState state, bool do_write = true) : base(state)
        {
            if (do_write)
            {
                Debug.WriteLine("Sending 'ID?'");
                this.Write("ID?");
            }
        }

        public override StartScriptState Handle(string a)
        {
            int type, id;

            Match m = Regex.Match(a, csvRegex_id);
            int.TryParse(m.Groups[1].Value, out type);
            int.TryParse(m.Groups[2].Value, out id);

            if (type == ID_LISTENER)
            {
                if (!this._sessions.ContainsKey(id))
                {
                    Debug.Write("Sessions does not Exist! :" + id + "\r\n");
                    this.Write("Sessions does not Exist! :" + id + "\r\n");
                    this._newSession.Disconnect();
                    return new WorkingState(this);  
                }
                else
                {
                    Debug.Write("Tapping into Session: " + id + "\r\n");
                    this.Write("Tapping into Session: " + id + "\r\n");
                    this._sessions[id].AddSession(ETelnetProxySession.Tap, this._newSession);
                    return new WorkingState(this);  
                }
            }
            else if (type == ID_NEWSESSION)
            {
                return new IdSessionState(this, id);
            }
            else
            {
                Debug.WriteLine("Bad response to 'ID?'");
                this.Write("Bad Response!\r\n");
                return new IdState(this);
            }
        }

        public override StartScriptState handle(ref byte[] buffer, ref int offset, ref int count)
        {
            if (buffer[offset] == 255 || buffer[offset] == (byte)'\r') {
                Debug.WriteLine("In ID State buf got telnet gunk!");
                var next_state = new TelnetState(this);
                return next_state.handle(ref buffer, ref offset, ref count);
            }
            
            if(count == 0)
            {
                return this;
            }
            //We got a buffer, we want to process from 0 to offset + count
            string line = ASCIIEncoding.ASCII.GetString(buffer, offset, buffer.Length - offset).Trim();
            
            int type, id, result = 0;
            //we got a response to "ID?"
            var new_state = this.Handle(line);
            if(new_state is IdState) {
                //in the same state - don't consume anything from the buffer, update the offset
                //clear the buffer instead
                buffer = new byte[4096];
                offset = 0;
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


    public class WorkingState : StartScriptState
    {
        public WorkingState(StartScriptState state) : base(state)
        {
        }

        public WorkingState(ITelnetSessionControl newSession, Dictionary<int, ITelnetProxySessionControl> sessions) : base(newSession, sessions)
        {
        }

        public override StartScriptState Handle(string a)
        {
            return this;
        }

        public override StartScriptState handle(ref byte[] buffer, ref int offset, ref int count)
        {
            throw new NotImplementedException();
        }
    }

    public class NewConnectionScript
    {
        private readonly Queue<string> _queue;
        private bool running = true;

        private StartScriptState _state;
        EventHandler<DataRcvEvent> ClientRcvdDataEventHandler;

        public NewConnectionScript()
        {
            ClientRcvdDataEventHandler = new EventHandler<DataRcvEvent>(newProxySession_ClientDataReceivedListener_Event);
        }

        public async Task<int> RunLogonScript(ITelnetSessionControl newTelnetSession, Dictionary<int, ITelnetProxySessionControl> sessions)
        {
            using (var networkStream = new NetworkStream(newTelnetSession.GetTcpClient().Client))
            using (var reader = new StreamReader(networkStream, Encoding.ASCII))
            {
                this._state = new TelnetState(newTelnetSession, sessions);

                byte[] buffer = new byte[4096];
                int offset = 0;
                int count = 0;
                while (!(this._state is WorkingState))
                {
                    count = await networkStream.ReadAsync(buffer, offset, buffer.Length);
                    //buffer[0 .. offset+count] is the stuff we need to parse
                    this._state = this._state.handle(ref buffer, ref offset, ref count);
                    offset = count;
                }

                //newTelnetSession.Receive_Event += ClientRcvdDataEventHandler;
                newTelnetSession.BeginRead();
            }

            return 1;
        }

        void newProxySession_ClientDataReceivedListener_Event(object sender, DataRcvEvent e)
        {
            //once we are setup this channel will recieve data coming from mega
            string s = ASCIIEncoding.ASCII.GetString(e.DataBuffer);
            if (e.DataBuffer[0] == 255)
            {
                byte[] telnet_iac = new byte[3];

                Buffer.BlockCopy(e.DataBuffer, 0, telnet_iac, 0, 3);
                string s2 = "";
                s2 += telnet_iac[0] + " ";
                s2 += telnet_iac[1] + " ";
                s2+= telnet_iac[2];
                Debug.WriteLine(s2);
                return;
            }

            Debug.WriteLine(s);
            int k = 1;
            //rcvd.Enqueue(s);
            //mre.Set();
        }
    }

}
