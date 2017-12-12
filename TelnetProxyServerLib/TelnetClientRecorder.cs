using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//namespace TelnetProxyServer
//{
//    public class TelnetClientRecorder
//    {
//        FileStream[] m_file;
//        StreamWriter[] m_writer;
//        public EventHandler<DataRcvEvent> DataRcvCallback;
//        public EventHandler DisconnectCallback;

//        const string p1 = "<Packet>";
//        const string p2 = @"<\Packet>\r\n";
//        object _lock = new object();

//        public TelnetClientRecorder(FileInfo fi)
//        {
//            int idx = fi.FullName.LastIndexOf(".");
//            if (idx == -1) idx = 0;
//            string fi2Path = fi.FullName.Substring(0, fi.FullName.Length - idx);
//            fi2Path += "_RAW.txt";
//            FileInfo fi2 = new FileInfo(fi2Path);

//            this.m_file = new FileStream[] { 
//                new FileStream(fi.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.Read),
//                new FileStream(fi2.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.Read)
//            };

//            this.m_writer = new StreamWriter[] {
//                new StreamWriter(this.m_file[0]),
//                new StreamWriter(this.m_file[1])
//            };

//            this.DataRcvCallback = new EventHandler<DataRcvEvent>(DataRcvMethod);
//            this.DisconnectCallback = new EventHandler(DisconnectMethod);
//        }

//        public void DataRcvMethod(object sender, DataRcvEvent e)
//        {

//            lock (this._lock)
//            {
//                this.m_writer[1].WriteLine(BitConverter.ToString(e.DataBuffer));
//                this.m_writer[1].Flush();

//                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(p1);
//                this.m_file[0].Write(buffer, 0, buffer.Length);
//                this.m_file[0].Write(e.DataBuffer, 0, e.DataBuffer.Length);
//                buffer = ASCIIEncoding.ASCII.GetBytes(p2);
//                this.m_file[0].Write(buffer, 0, buffer.Length);
//                this.m_file[0].Flush();
//            }
//        }

//        public void DisconnectMethod(object sender, EventArgs e)
//        {
//            this.m_writer[0].Close();
//            this.m_writer[1].Close();
//        }
//    }
//}
