// See https://aka.ms/new-console-template for more information
using TelnetProxyServer;
using TelnetProxyServer.TelnetClient;

Console.WriteLine("Hello, World!");
var cli = new Client();
cli.start("bbs.classicmud.com", 2323);
Thread.Sleep(100000);

public class Client
{
    public Client() { }

    EventHandler<DataRcvEvent> rcvr;

    public void start(string ip, int port)
    {
        TelnetSession session = new TelnetSession(ip, port);
        rcvr = new EventHandler<DataRcvEvent>(reciever);
        session.Receive_Event += rcvr;

        session.Connect();

        byte[] foo = new byte[] { 255,
            253,
            3,
        };
    }

    public void reciever(object sender, DataRcvEvent e)
    {
        Console.WriteLine(e.ToString());
        byte[] foo = new byte[] { 255,
            253,
            3,
        };
        (sender as TelnetSession).SendToRemote(foo);
    }
}


