using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm_Protocols
{
    public delegate void RcvMsgCallback(byte[] buffer);

    public interface IConnection
    {
        bool Connect();
        void Disconnect();

        bool isConnected();
        
        event RcvMsgCallback Rcvr;
        event EventHandler Disconnected;
        void Send(byte[] buffer);
        
    }
}
