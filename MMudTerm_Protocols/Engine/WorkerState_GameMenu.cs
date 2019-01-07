using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    internal class WorkerState_GameMenu : WorkerState
    {
        public bool EnterGame = true;
        //while in the logon state I look for these regex's in this order
        List<RegexAndResponse> RegexAndResponses;
        public WorkerState_GameMenu()
        {
            RegexAndResponses = new List<RegexAndResponse>(MyRegex.Cache[this.GetType()]);
        }

        internal override int FlushCmds()
        {
            throw new System.NotImplementedException();
        }

        internal override WorkerState HandleTermCmd(Engine eng, TermCmd cmd)
        {
            if (!(cmd is TermStringDataCmd)) return this;

            foreach (RegexAndResponse rr in this.RegexAndResponses)
            {
                if (!rr.IsMatch(cmd as TermStringDataCmd)) continue;

                if (this.EnterGame) {
                    eng.Send(rr.Reponse);
                    return new WorkerState_InGame();
                }
            }
            return base.HandleTermCmd(eng, cmd);
        }
    }
}