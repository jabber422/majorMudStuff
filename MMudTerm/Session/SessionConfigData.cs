using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace MMudTerm
{
    [Serializable]
    public class SessionConnectionInfo
    {
        public string Name { get; set; }

        private string _ip;
        public string Ip {
            get { return this._ip; } set {
                this._ip = Dns.GetHostAddresses(value)[0].ToString();
            } }

        public short Port { get; set; }

        public int Rows { get; set; }

        public int Cols { get; set; }

        public bool AutoConnect { get; set; }

        public IPAddress IpA{ get{ return IPAddress.Parse(this.Ip); } }

        public List<Tuple<string, string>> LogonAutomation { get; set; }
        public int BbsControlId { get; internal set; }
    }
}
