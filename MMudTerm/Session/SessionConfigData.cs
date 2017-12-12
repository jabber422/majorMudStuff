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

        public string Ip { get; set; }

        public short Port { get; set; }

        public int Rows { get; set; }

        public int Cols { get; set; }

        public bool AutoConnect { get; set; }

        public IPAddress IpA{ get{ return IPAddress.Parse(this.Ip); } }
    }
}
