using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_Matching : GameProcessorState
    {
        List<MatchAndCapture> ThingsThisColorWillMatchAgainst
        {
            get
            {
                if (MatchAndCaptureTables.Cache2.ContainsKey(this.CurrentColorAttribute))
                {
                    if (MatchAndCaptureTables.Cache2[this.CurrentColorAttribute].ContainsKey(this.CurrentColorForeGround))
                    {
                        return MatchAndCaptureTables.Cache2[this.CurrentColorAttribute][this.CurrentColorForeGround];
                    }
                    else
                    {
                        Log.Tag(this.Tag, "No MatchAndCapture's for Attrib: " + this.CurrentColorAttribute + ", ForeGround: " + this.CurrentColorForeGround);
                    }
                }
                else
                {
                    Log.Tag(this.Tag, "No MatchAndCapture's for Attrib: " + this.CurrentColorAttribute);
                    throw new Exception("Attrib not 0 or 1, need to implement this");
                }
                return new List<MatchAndCapture>();
            }
        }

        public GameProcessorState_Matching(GameProcessorState state = null) : base(state)
        {
            Log.Tag(this.Tag, "New GameProcessorState created -> " + this.GetType().Name);
        }

        internal override GameProcessorState HandleAnsiGraphicsCmd(WorkerState_InGame workerState, AnsiGraphicsCmd ansiGraphicsCmd)
        {
            if (this.CurrentColorAttribute != ansiGraphicsCmd.Attribute || this.CurrentColorForeGround != ansiGraphicsCmd.ForeGround)
            {
                Log.Tag(this.Tag , "Color change - from {0};{1} to {2};{3}", this.CurrentColorAttribute.ToString(),
                    this.CurrentColorForeGround.ToString(), ansiGraphicsCmd.Attribute.ToString(), ansiGraphicsCmd.ForeGround.ToString());

                if (this.CurrentColorAttribute != ansiGraphicsCmd.Attribute)
                    this.CurrentColorAttribute = ansiGraphicsCmd.Attribute;
                if(this.CurrentColorForeGround != ansiGraphicsCmd.ForeGround)
                    this.CurrentColorForeGround = ansiGraphicsCmd.ForeGround;
            }
            return this;
        }

        internal override GameProcessorState HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd cmd)
        {
            if (this.ThingsThisColorWillMatchAgainst == null) return this;

            string stringCmd = cmd.GetValue();
            Log.Tag(this.Tag, "{0} >" + stringCmd + "<", this.GetType().ToString());

            foreach (MatchAndCapture mac in this.ThingsThisColorWillMatchAgainst)
            {
                if (mac.IsMatch(stringCmd))
                {
                    this.MatchAndCapture = mac;
                    this.MatchAndCapture.Consume(stringCmd);
                    return new GameProcessorState_Capturing(this);
                    //ConsumeResults result = mac.Consume(stringCmd);
                    //Log.Tag(this.Tag, "Consume result -> " + result.ToString());
                    //if (result == (ConsumeResults.MatchComplete & ConsumeResults.CaptureComplete))
                    //{   
                    //    //matched and captured in the same string, send the match up and return the same state
                    //    workerState.Engine.GameDataChange(mac.CreateDataChangeItem());
                    //    return this;
                    //}
                    //else if (result == ConsumeResults.Capturing)
                    //{
                    //    this.MatchAndCapture = mac;
                    //    //have a match but it's still looking for more data, put us in the capturing state
                    //    GameProcessorState state = new GameProcessorState_Capturing(this);
                    //    state.CurrentColorAttribute = this.CurrentColorAttribute;
                    //    state.CurrentColorForeGround = this.CurrentColorForeGround;
                    //    return state;
                    //}
                }
            }

            Log.Tag(this.Tag, "Unmatched string -> " + stringCmd);
            return this;
        }
    }

}


