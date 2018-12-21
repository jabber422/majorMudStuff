using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm_Connection
{
    public delegate void RcvMsgCallback(byte[] buffer);

    public interface IConnection
    {
        bool Connect();
        bool Disconnect();

        bool Connected {get;}
        
        event RcvMsgCallback Rcvr;
        event EventHandler Disconnected;
        int Send(byte[] buffer);
        
    }
}
