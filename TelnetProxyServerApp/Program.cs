using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using TelnetProxyServer;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Debug.AutoFlush = true;
                //aceListener tl = Trace.
                ConsoleTraceListener ctl = new ConsoleTraceListener();
                Debug.Listeners.Add(ctl);

                TelnetProxyServerMain foo = new TelnetProxyServerMain(IPAddress.Any, 12345);
                foo.Start();

                Console.Read();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
