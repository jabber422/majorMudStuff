using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MMudTerm_Connection
{
    public static class ConnFactory
    {
        static public IConnection CreateNewConnection(IPAddress address, int port)
        {
            return new ConnObj(address, port);
        }
    }
}
