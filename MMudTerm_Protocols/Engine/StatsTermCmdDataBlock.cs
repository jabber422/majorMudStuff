using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MMudTerm_Protocols.Engine
{
    internal class StatsTermCmdDataBlock : TermCmdDataBlock
    {
        PlayerStats newStats = new PlayerStats();
        public StatsTermCmdDataBlock(TermCmdDataBlock block) : base(block.cmds)
        {
            //validate the block
            if (!(this.cmds[2] is TermStringDataCmd stringCmd))
            {
                throw new Exception("Invalid Stats Block");
            }
            else if(!stringCmd.GetValue().Contains("Name"))
            {
               throw new Exception("Invalid Stats Block");
            }
            else if(!(this.cmds[0] is AnsiGraphicsCmd)  && !(this.cmds[1] is AnsiGraphicsCmd))
            {
                throw new Exception("Invalid Stats Block");
            }

            //something has decided that this block is a stats block
            //basically a bunch of key value pair with color changes
            //the last string is option and it he list of buffs as a sing string i think?
            string Key = "";
            foreach (TermCmd cmd in this.cmds)
            {
                if (cmd is TermStringDataCmd stringDataCmd)
                {
                    if (Key == "")
                    {
                        Key = stringDataCmd.GetValue().Trim();
                        Key = Key.TrimEnd(':');
                    }
                    else
                    {
                        Key = Key.Replace('/', '_');
                        Key = Key.Replace(' ', '_');
                        if (stringDataCmd.GetValue() == "*") continue;
                        PropertyInfo pi = newStats.GetType().GetProperty(Key);
                        string Value = stringDataCmd.GetValue().Trim();
                        object propertyValue = Convert.ChangeType(Value, pi.PropertyType);
                        pi.SetValue(newStats, propertyValue, null);
                        Key = "";
                    }
                }
            }

            if(Key != "")
            {
                //todo
                //but wait there is more
                //this should be party and buffs info
            }
        }

        internal override void UpdateModel(Engine engine)
        {
            engine.Player.Stats.Update(this.newStats);
        }

        internal override bool ValidateBlock(TermCmdDataBlock block)
        {
            if (!(block.cmds[0] is AnsiGraphicsCmd ansiCmd))
            {
                return false;
            }
            else if (ansiCmd.Attribute != ANSI_COLOR.All_off || ansiCmd.ForeGround != ANSI_COLOR.Green)
            {
                return false;
            }
            else if (!(block.cmds[1] is TermStringDataCmd stringCmd))
            {
                return false;
            }
            else if (stringCmd.GetValue() != "Name:")
            {
                return false;
            }

            return true;
        }
    }

    internal class HealthUpdateCmdDataBlock : TermCmdDataBlock
    {
        //[AnsiGraphicsCmd: 0, [AnsiGraphicsCmd: 0, 36]
        //[TermStringDataCmd]: [HP=
        //[AnsiGraphicsCmd: 1, 36]
        //[TermStringDataCmd]: 668
        //[AnsiGraphicsCmd: 0, 36]
        //[TermStringDataCmd]:  (Resting) ]:
        //[AnsiCursorBkwdCmd: cols 79]
        //[AnsiEraseDisplayCmd]
        int hits = int.MinValue;
        int mana = int.MinValue;
        bool resting = false;
        bool meditating = false;

        public HealthUpdateCmdDataBlock(TermCmdDataBlock block) : base(block.cmds)
        {
            if (!ValidateBlock(block))
            {
                throw new Exception("Invalid HealthUpdateCmdDataBlock format");
            }

            this.hits = int.Parse((this.cmds[3] as TermStringDataCmd).GetValue().Trim());
            Log.Debug("Hits = " + hits);

            string s = (this.cmds[5] as TermStringDataCmd).GetValue().Trim();
            if (s.StartsWith("/MA="))
            {
                this.mana = int.Parse((this.cmds[7] as TermStringDataCmd).GetValue().Trim());

                s = (this.cmds[9] as TermStringDataCmd).GetValue().Trim();
                if (s.Contains("Resting"))
                {
                    this.resting = true;
                }
                else if (s.Contains("Meditating"))
                {
                    this.meditating = true;
                }
            }
            else if (s.Contains("Resting"))
            {
                this.resting = true;
            }else if( s.Contains("Meditating")){
                this.meditating = true;
            }
        }

        internal override void UpdateModel(Engine engine)
        {
            if (this.mana != int.MinValue)
                engine.Player.Stats.UpdateHealth(this.hits, this.mana);
            else
                engine.Player.Stats.UpdateHealth(this.hits);

            engine.Player.IsResting = this.resting;
            engine.Player.IsMeditating = this.meditating;
        }

        internal override bool ValidateBlock(TermCmdDataBlock block)
        {
            if (!(block.cmds[0] is AnsiGraphicsCmd ansiCmd))
            {
                return false;
            }
            else if (ansiCmd.Attribute != ANSI_COLOR.All_off || ansiCmd.ForeGround != ANSI_COLOR.Cyan)
            {
                return false;
            }
            else if (!(block.cmds[1] is TermStringDataCmd stringCmd))
            {
                return false;
            }
            else if(stringCmd.GetValue() != "[HP=")
            {
                return false;
            }

            return true;
        }
    }

    internal class ObviousItemsCmdDataBlock : TermCmdDataBlock
    {
        public ObviousItemsCmdDataBlock(List<TermCmd> myBlock) : base(myBlock)
        {
        }

        internal override void UpdateModel(Engine engine)
        {
            throw new NotImplementedException();
        }

        internal override bool ValidateBlock(TermCmdDataBlock block)
        {
            throw new NotImplementedException();
        }
    }
}