using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    //in this state we execute the logon script
    public class WorkerState_Logon : WorkerState
    {
        //while in the logon state I look for these regex's in this order
        List<RegexAndResponse> RegexAndResponses;
        public WorkerState_Logon()
        {
            RegexAndResponses = new List<RegexAndResponse>(MyRegex.Cache[this.GetType()]);
        }

        internal override int FlushCmds()
        {
            return -1;
        }

        internal override WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            if (!eng.ConnObj.Connected)
                return new WorkerState_ThreadRunning();

            if (!(cmd is TermStringDataCmd)) return this;

            
            foreach(RegexAndResponse rr in this.RegexAndResponses)
            {
                if (rr.IsMatched) continue;
                if (!rr.IsMatch(cmd as TermStringDataCmd)) continue;

                eng.ConnObj.Send(ASCIIEncoding.ASCII.GetBytes(rr.Reponse));
                rr.IsMatched = true;
                break;
            }

            bool done = true;
            foreach (RegexAndResponse rr in this.RegexAndResponses)
            {
                done &= rr.IsMatched;
            }

            if (done)
            {
                return new WorkerState_GameMenu();
            }

            return base.HandleTermCmd(eng, cmd);
        }
    }
}
