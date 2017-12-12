using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TelnetProxyServer.TelnetClient;

namespace TelnetProxyServer.TelnetProxy
{
    /// <summary>
    /// A telnet proxy session will have one "local" client and one "remote" client, client = TelnetSession class
    /// local is the megamud running on your computer
    /// remote is the game server
    /// provides hooks for I/O to either stream at the packet level
    /// </summary>
    public interface ITelnetProxySessionControl : IDisposable
    {
        event EventHandler DisconnectEvent;

        ITelnetSessionControl ClientSession { get; }
        ITelnetSessionControl RemoteSession { get; }
        ITelnetSessionControl TapSession { get; }

        //the game server sent something
        //event EventHandler<DataRcvEvent> RemoteDataReceivedListener_Event;
        ////the local megamud sent something
        //event EventHandler<DataRcvEvent> ClientDataReceivedListener_Event;
        //event EventHandler<DataRcvEvent> TapDataReceivedListener_Event;
        
        //send something to the local
        void RcvFromRemoteSession(object sender, DataRcvEvent e);
        void RcvFromClientSession(object sender, DataRcvEvent e);
        void RcvFromTapSession(object sender, DataRcvEvent e);

        int SendTo(ETelnetProxySession eClientType, string str);

        int GetId { get; }

        void AddSession(ETelnetProxySession eTelnetProxySession, ITelnetSessionControl session);
        void RemoveSession(ETelnetProxySession eTelnetProxySession);
        void DisconnectSession(ETelnetProxySession eTelnetProxySession);
    }
}
