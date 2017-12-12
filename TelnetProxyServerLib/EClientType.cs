using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelnetProxyServer
{
    public enum EClientType
    {
        Local,Remote,Listener
    }

    public enum ETelnetProxySession
    {
        NULL = 0, Remote, Both,
        Client,
        Tap
    }

    public enum ETelnetProxyServerStatus
    {
        NULL = 0, STOPPED, STARTING, RUNNING, ERROR
    }
}
