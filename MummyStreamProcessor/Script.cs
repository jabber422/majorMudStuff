using MMudTerm.Connection;
using MMudTerm_Protocols;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace MummyStreamProcessor
{
    class Script
    {
        Object m_stopping = true;
        Boolean Stopped = true;
        Boolean Run = false;

        ConnObj m_connObj = null;
        ProtocolDecoder myDecoder = null;
        BackgroundWorker Worker = null;
        Queue<String> Messages = null;
        Object MessageLock = new object();

        //Game states
        bool IsInCombat = false;
        bool IsResting = false;
        double RestBelow = .6;

        System.Timers.Timer MyIdleTimer;

        public Script(ConnObj connObj)
        {
            this.Messages = new Queue<string>();
            this.m_connObj = connObj;
            this.m_connObj.Rcvr += new RcvMsgCallback(ConnHandler_Rcvr);
            this.Worker = new BackgroundWorker();
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerAsync();
            this.MyIdleTimer = new System.Timers.Timer(5 * 1000);
            this.MyIdleTimer.Elapsed += MyIdleTimer_Elapsed;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Run = true;
            while (this.Run)
            {
                while (Messages.Count > 0)
                {
                    String msg = "";
                    lock (this.MessageLock)
                    {
                        msg = Messages.Dequeue();
                    }

                    DoWork(msg);
                }
                Thread.Sleep(10);
            }
            Debug.WriteLine("Worker thread is stopping");
        }

        String[] delim = new string[] { "\r\n" };

        internal void DoWork(string text)
        {
            Console.WriteLine(text);

            String[] lines = text.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (String line in lines)
            {
                if (line.Contains("ID?"))
                {
                    Send("ID,2,42");
                }

                if (line.Contains("*Combat Engaged*"))
                {
                    this.IsInCombat = true;
                    this.MyIdleTimer.Enabled = false;
                } else if (line.Contains("*Combat Off*")) {
                    this.IsInCombat = false;
                    this.MyIdleTimer.Enabled = true;
                }

                Regex isMummyHere = new Regex(@".*(mummy|ghoul|shade|skeleton|zombie).");


       
                foreach (Match match in isMummyHere.Matches(line))
                {
                    if (match.Success && !this.IsInCombat)
                    {
                        if (line.Contains("aa " + match.Groups[1].Value))
                        { continue; }
                        String monster = match.Groups[1].Value;
                        Send(string.Format("aa {0}\n", monster));
                        Send(string.Format("/caslus @do aa {0}\n", monster));
                    }
                }
            }
        }

        private void MyIdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Send("s\n");
            this.Send("n\n");
        }

        private void Send(string v)
        {
            this.m_connObj.Send(ASCIIEncoding.ASCII.GetBytes(v));
        }

        private void ConnHandler_Rcvr(byte[] buffer)
        {
            if (buffer.Length == 0)
                return; //TODO: buffer of zero means a disconnect?
            this.myDecoder = new AnsiProtocolDecoder();
            ProtocolCommand cmd = this.myDecoder.DecodeBuffer(buffer);
            String text = cmd.ToString();

            if (!text.Equals(String.Empty))
            {
                lock (this.MessageLock)
                {
                    this.Messages.Enqueue(text);
                }
            }
        }

        internal void Stop()
        {
            if (!this.Run) return;
            lock (this.m_stopping)
            {
                this.Run = false;
                if (this.Worker.IsBusy)
                {
                    this.Worker.CancelAsync();
                }
            }
        }
    }

    public class InCombat
    {
        DateTime LastInCombatTime;
        DateTime LastOutOfCombatTime;
        bool _IsInCombat = false;

        public bool IsInCombat {
            get { return this._IsInCombat; }
            set {

                this._IsInCombat = value; }
        }
}
}
