using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace TelnetProxyServer.TelnetClient
{
    public interface ITelnetSessionControl
    {
        bool IsConnected { get; }
        Guid Id { get; }
        string IpAddress { get; }
        int Port { get; }

        string Name { get; set; }
        //sends a raw byte stream to the remote client
        int SendToRemote(byte[] buffer);
        //sends a raw byte stream to the local client
        //loops back to the rcvdata event
        //lets you display something to the local client
        int SendToLocal(byte[] buffer);
        //fire when data is rcv from the remote client
        event EventHandler<DataRcvEvent> Receive_Event;
        //fired when data is rcv from the local client
        //event EventHandler<DataRcvEvent> RcvFromLocal_Event;

        event EventHandler Disconnect_Event;

        void SetTcpClient(TcpClient client);

        bool Connect();
        bool Disconnect();
        void BeginRead();

    }
}
