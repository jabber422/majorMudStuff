using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
//using MMudTerm.Session;

namespace MMudTerm_Protocols
{
    /// <summary>
    /// Our connection Socket
    /// TODO: this is quick and dirty, needs a lot of work
    /// </summary>
    public static class SocketHandler
    {
        static AsyncCallback EndConnCB;
        static AsyncCallback EndRcvCB;
        static AsyncCallback EndSendCB;

        //public event EventHandler Disconnected;
        //public event RcvMsgCallback Rcvr;

        static SocketHandler()
        {
            EndConnCB = new AsyncCallback(EndConnect);
            EndRcvCB = new AsyncCallback(EndRcv);
            EndSendCB = new AsyncCallback(EndSend);
        }

        #region public methods
        static public bool Connect(ConnObj con)
        {
            if (!con.Connected)
            {
                try
                {
                    IAsyncResult ar =
                        con.mySocket.BeginConnect(con.Ip, con.Port, EndConnCB, con);
                    ar.AsyncWaitHandle.WaitOne();
                }catch(Exception ex)
                {
                    Trace.WriteLine("SocketHandler.Connect ex!");
                    Trace.WriteLine(ex.ToString());
                }
            }
            return con.Connected;
        }

        static public void Disconnect(ConnObj con)
        {
            if (con.Connected)
            {
                con.mySocket.Shutdown(SocketShutdown.Both);
                con.mySocket.Close();
            }
            //con.Disconnect
        }

        static public bool Send(ConnObj con, byte[] buffer)
        {
            if(con.Connected)
            {
                con.mySocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None,
                    EndSendCB, con);
                return true;
            }
            return false;
        }

        static public void BeginReceive(ConnObj con)
        {
            try
            {
                con.mySocket.BeginReceive(
                            con.Buffer, 0, con.Buffer.Length,
                            SocketFlags.None, EndRcvCB, con);
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region private parts
        static private void EndConnect(IAsyncResult ar)
        {
            ConnObj con = (ConnObj)ar.AsyncState;
            con.mySocket.EndConnect(ar);

            if (con.mySocket.Connected)
            {
                try
                {
                    BeginReceive(con);
                }
                catch (SocketException se)
                {
                }
            }
        }

        static private void EndRcv(IAsyncResult ar)
        {
            ConnObj con = (ConnObj)ar.AsyncState;
            
            int size;
            try
            {
                size = con.mySocket.EndReceive(ar);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 0x00002746)  //server forced a socket closed
                {
                    con.BroadcastDisconnected();
                }
                else
                {
                    Trace.WriteLine("SocketHandler.EndRcv - caught socket ex! err.code = " + ex.ErrorCode);
                    Trace.WriteLine(ex.ToString());
                }
                return;
            }
            catch (InvalidOperationException ex)
            {
                return;
            }

            byte[] buffer = new byte[size];
            Buffer.BlockCopy(con.Buffer, 0, buffer, 0, size);
            con.BroadcastRcv(buffer);

            try
            {
                BeginReceive(con);
            }
            catch (SocketException se)
            {
                if (se.SocketErrorCode == SocketError.Shutdown || 
                    se.SocketErrorCode == SocketError.ConnectionAborted)
                {
                    con.BroadcastDisconnected();
                } 
            }
        }

        static private void EndSend(IAsyncResult ar)
        {
            ConnObj obj = (ConnObj)ar.AsyncState;
            obj.mySocket.EndSend(ar);
        }
        #endregion
    }
}
