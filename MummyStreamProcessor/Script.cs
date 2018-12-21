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

        public Script(ConnObj connObj)
        {
            this.Messages = new Queue<string>();
            this.m_connObj = connObj;
            this.m_connObj.Rcvr += new RcvMsgCallback(ConnHandler_Rcvr);
            this.Worker = new BackgroundWorker();
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerAsync();
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

        internal void DoWork(string text)
        {
            Console.WriteLine("-> " + text + " <-");
            if (text.Contains("ID?"))
            {
                Send("ID,2,42");
            }

            if (text.Contains("*Combat Engaged*"))
            {
                this.IsInCombat = true;
            } else if (text.Contains("*Combat Off *")) {
                this.IsInCombat = false;
            }

            Regex isMummyHere = new Regex(@"Also here:.*mummy\.|\,");

            foreach(Match match in isMummyHere.Matches(text))
            {
                if(match.Success && !this.IsInCombat)
                {
                    Send("aa mummy");
                }
            }
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
            Queue<TermCmd> cmds = this.myDecoder.DecodeBuffer(buffer);
            foreach (TermCmd c in cmds)
            {
                if (c is TermStringDataCmd)
                {
                    if (!(c is TermStringDataCmd)) continue;

                    String text = (c as TermStringDataCmd).GetValue();
                    lock (this.MessageLock) {
                        this.Messages.Enqueue(text);
                    }
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
