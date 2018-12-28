using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MMudTerm_Protocols
{
    public class ProtocolConnObj : ConnObj
    {
        public ProtocolConnObj(IPAddress ip, int port) : base(ip, port)
        {
        }



        //incoming raw buffer from the server socket
        ///override public void BroadcastRcv(byte[] buffer)
      //  {
           // if (.Rcvr != null)
         //   {
            //    Trace.WriteLine("{0} -> Send()", this.Name);
               // Rcvr(buffer);
          //  }
       // }
    }
}
