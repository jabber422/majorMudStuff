using MMudObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace MMudTerm_Protocols
{
    //public delegate void voidFuncString(string s);

    //base class for a protocole decoder
    public abstract class ProtocolDecoderV2
    {
        //stored buffer[] -> TermCmd conversion, consumed by user thread
        internal Queue<TermCmd> TermCmdsQueue;
        protected List<byte[]> values;
        protected Stack<byte> pieces;
        protected byte[] partialMsgBuffer;
        private ConnObj m_connObj;

        public ManualResetEvent mre = new ManualResetEvent(false);

        protected object InUse = new object();
        
        //ctor
        protected ProtocolDecoderV2(ConnObj connObj)
        {
            Log.Enter();

            TermCmdsQueue = new Queue<TermCmd>();
            values = new List<byte[]>();
            pieces = new Stack<byte>();
            partialMsgBuffer = new byte[0];

            this.m_connObj = connObj;
            this.m_connObj.Rcvr += connObj_Rcvr;
            Log.Exit();
        }

        //rcvr for the connobj.Rcvr event, use it's thread, lock the decoder, process the buffer into TermCmds
        public void connObj_Rcvr(byte[] buffer)
        {
            if (buffer.Length == 0) return;
            string buffStr = Encoding.ASCII.GetString(buffer);
            lock (this.InUse)
            {
                this.DecodeBuffer(buffer);
            }
            this.mre.Set();
        }

        //called by the ConnObj rcvr thread, this converts the raw byte[] to useable cmds
        protected abstract void DecodeBuffer(byte[] buffer);

        

        //either of these empty our queue
        public List<TermCmd> GetTermCmds()
        {
            lock (this)
            {
                List<TermCmd> result = new List<TermCmd>();
                while (this.TermCmdsQueue.Count > 0)
                {
                    result.Add(this.TermCmdsQueue.Dequeue());
                }
                this.mre.Reset();
                return result;
            }
        }

        public void AddTermCmd(TermCmd cmd)
        {
            this.TermCmdsQueue.Enqueue(cmd);
        }

        /// <summary>
        /// Appends the partial msg buffer to the head of the new buffer
        /// </summary>
        /// <param name="buffer">rcvd buffer</param>
        /// <returns>complete buffer</returns>
        protected byte[] ConcatBuffers(byte[] buffer)
        {
            if (partialMsgBuffer.Length == 0)
                return buffer;
            
            string s = "";
            #region debug
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
            Debug.Indent();

            s = String.Format("partialBuffer.Len={0}, buffer.Len={1}", 
                partialMsgBuffer.Length, buffer.Length);
            Debug.WriteLine(s, this.GetType().Namespace);
           
            s = null;
            for (int i = 0; i < partialMsgBuffer.Length; ++i)
            {
                s += String.Format("{0:X}-", partialMsgBuffer[i]);
            }
            Debug.WriteLine("pmb contains: " + s, this.GetType().Namespace);
            
            s = null;
            for (int i = 0; i < buffer.Length; ++i)
            {
                s += String.Format("{0:X}-",buffer[i]);
            }
            Debug.WriteLine("buf contains: " + s, this.GetType().Namespace);
            s = null;
#endif
            #endregion
            byte[] temp = new byte[partialMsgBuffer.Length + buffer.Length];
            System.Buffer.BlockCopy(partialMsgBuffer, 0, temp, 0, partialMsgBuffer.Length);
            System.Buffer.BlockCopy(buffer, 0, temp, partialMsgBuffer.Length,
                temp.Length - partialMsgBuffer.Length);
            
            partialMsgBuffer = new byte[0];
            #region debug
#if DEBUG
            
            for (int i = 0; i < temp.Length; ++i)
            {
                s += String.Format("{0:X}-", temp[i]);
            }
            Debug.WriteLine("concated contains: " + s, this.GetType().Namespace);
            Debug.Unindent();
#endif
            #endregion
            return temp;
        }

        /// <summary>
        /// saves any partial commands
        /// </summary>
        /// <param name="buffer">the buffer</param>
        /// <param name="fromIdx">index to start the save at</param>
        protected void SaveBuffer(byte[] buffer, int fromIdx)
        {
            #region debug 
            string s = "";
#if DEBUG_2
            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
            Debug.Indent();
            s = String.Format("buffer.Len={0}, idx={1}", buffer.Length, fromIdx);
            Debug.WriteLine(s, this.GetType().Namespace);

            s = null;
            for (int i = fromIdx; i < buffer.Length; ++i)
            {
                s += String.Format("{0:X}-", buffer[i]);
            }
            Debug.WriteLine("buffer fromIdx contains: " + s, this.GetType().FullName);
            s = null;
#endif
            #endregion
            partialMsgBuffer = new byte[buffer.Length - fromIdx];
            System.Buffer.BlockCopy(buffer, fromIdx, partialMsgBuffer, 0, partialMsgBuffer.Length);
            #region debug
#if DEBUG
            
            for (int i = 0; i < partialMsgBuffer.Length; ++i)
            {
                s += String.Format("{0:X}-", partialMsgBuffer[i]);
            }
            Debug.WriteLine("pmb contains: " + s, this.GetType().Namespace);
            Debug.Unindent();
#endif
            #endregion
        }
   }
}
