using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;

namespace TelnetProxyServer
{
    public class MyBuffer : EventArgs, ICloneable
    {
        public byte[] DataBuffer { get; set; }
        public int Size { get; set; }
        public int Head { get; set; }
        //head + size
        public int Tail
        {
            get
            {
                var retVal = this.Size + this.Head;
                if (retVal > this.DataBuffer.Length)
                    throw new ArgumentOutOfRangeException("Buffers are too small?");
                return retVal;
            }
        }
        //Len - Tail
        public int SizeLeft
        {
            get
            {
                int retVal = this.DataBuffer.Length - this.Tail;
                if (retVal < 0)
                    throw new ArgumentOutOfRangeException("Buffers are too small? sizeleft");
                return retVal;
            }
        }

        public MyBuffer()
        {
            this.DataBuffer = new byte[256 * 32];
            this.Clear();
        }

        public MyBuffer(byte[] buffer)
        {
            this.DataBuffer = buffer;
            this.Size = buffer.Length;
            this.Head = 0;
        }

        public void Add(MyBuffer addBuffer)
        {
            lock (this.DataBuffer)
            {
                if (this.Tail + addBuffer.Size > this.DataBuffer.Length)
                {
                    throw new ArgumentOutOfRangeException("buffers are too small?");
                }
                Buffer.BlockCopy(addBuffer.DataBuffer, addBuffer.Head, this.DataBuffer, this.Tail, addBuffer.Size);
            }
            this.Size += addBuffer.Size;
        }

        public byte[] Take()
        {
            byte[] retVal = new byte[this.Size];
            lock (DataBuffer)
            {
                Buffer.BlockCopy(this.DataBuffer, this.Head, retVal, 0, this.Size);
                this.Clear();
            }
            return retVal;
        }

        public byte[] Get()
        {
            byte[] retVal = new byte[this.Size];
            Buffer.BlockCopy(this.DataBuffer, this.Head, retVal, 0, this.Size);
            return retVal;
        }

        public void Clear()
        {
            this.DataBuffer.Initialize();
            this.Size = 0;
            this.Head = 0;
        }

        public object Clone()
        {
            MyBuffer clone = new MyBuffer();
            clone.DataBuffer = (byte[])this.DataBuffer.Clone();
            clone.Head = this.Head;
            clone.Size = this.Size;
            return clone;
        }
    }

    public class DataRcvEvent : EventArgs
    {
        static UInt64 m_Counter = UInt64.MinValue;
        static UInt64 GetCounterId()
        {
            var cnt = m_Counter;
            if (m_Counter == UInt64.MaxValue)
            {
                m_Counter = UInt64.MinValue;
            }
            else
            {
                m_Counter++;
            }
            return cnt;
        }

        public UInt64 CountId { get; private set; }
        

        public byte[] DataBuffer { get; private set; }
        public DataRcvEvent(byte[] buffer)
        {
            this.CountId = GetCounterId();
            this.DataBuffer = buffer;
        }
    }

    public class SessionsDisconnectEvent : EventArgs
    {
        public string msg { get; private set; }
        public SessionsDisconnectEvent(string str)
        {
            this.msg = str;
        }
    }
}
