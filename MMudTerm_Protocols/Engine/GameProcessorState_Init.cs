using MMudObjects;
using System.Collections.Generic;
using System.Timers;

namespace MMudTerm_Protocols.Engine
{
    //internal class GameProcessorState_Init : GameProcessorState
    //{
        

    //    //the first state, we need to build a player object before any other states get handled.
    //    internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
    //    {
    //        if (workerState.Engine.Player != null)
    //            throw new System.Exception("This should not be called when a player object exists!");

    //        cmd = workerState.BufferTermCmd(cmd);
    //        workerState.Engine.Send("status");
    //        //return new GameProcessorState_GettingStats();
    //        return this;
    //    }
    //}

    //internal class GameProcessorState_GettingStats : GameProcessorState
    //{
    //    Dictionary<string, bool> shitToFind;
    //    bool timeoutFlag = false;
    //    Timer timeout;

    //    ColorKeyColorValue SubState;

    //    public GameProcessorState_GettingStats()
    //    {
    //        this.shitToFind = new Dictionary<string, bool>();

    //        this.timeout = new Timer(2 * 1000);
    //        this.timeout.Elapsed += Timeout_Elapsed;
    //        this.timeout.Enabled = true;

    //        SubState = new ColorKeyColorValue_StatsKey();
    //    }

    //    private void Timeout_Elapsed(object sender, ElapsedEventArgs e)
    //    {
    //        this.timeoutFlag = true;
    //    }

    //    public override GameProcessorState HandleTermCmd(Engine eng, TermCmd cmd)
    //    {
    //        if (timeoutFlag)
    //        {
    //            Log.Tag("GamProcessorState", "Getting stats has timed out");
    //            return new GameProcessorState_Init();
    //        }

    //        this.SubState = this.SubState.DoWork(eng, cmd);

    //        if (cmd is AnsiProtocolCmds.AnsiGraphicsCmd)
    //        {
    //            (cmd as AnsiProtocolCmds.AnsiGraphicsCmd).
    //        }
    //        if (cmd is TermStringDataCmd)
    //        {
    //            string line = (cmd as TermStringDataCmd).GetValue();
    //            if (line.Contains("Name:"))
    //            {

    //            }
    //        }

    //        cmd = eng.Decoder.Buffer(cmd);
    //        throw new System.NotImplementedException();
    //    }
    //}

    ////waiting for the green color ansi markup, when seen, move to getting key, after key move to value color, value etc...
    //internal class GameProcessorState_StatsGettingKeyColor : GameProcessorState
    //{
    //    //we are looking for green then a string with :
    //    public override GameProcessorState HandleTermCmd(Engine eng, TermCmd cmd)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    //internal class GameProcessorState_StatsGettingKey : GameProcessorState
    //{
    //    //we are looking for green then a string with :
    //    public override GameProcessorState HandleTermCmd(Engine eng, TermCmd cmd)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}

    //internal class GameProcessorState_StatsGettingValue : GameProcessorState
    //{
    //    public override GameProcessorState HandleTermCmd(Engine eng, TermCmd cmd)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}



//Name: Darmius Senru                    Lives/CP:      9/5
//Race: Dwarf Exp: 96269611        Perception:     37
//Class: Warrior Level: 40            Stealth:         0
//Hits:   583/640   Armour Class:  65/18 Thievery:        0
//                                       Traps:           0
//                                       Picklocks:       0
//Strength:  135    Agility: 30          Tracking:        0
//Intellect: 40     Health:  130         Martial Arts:    4
//Willpower: 50     Charm:   40          MagicRes:       57
//You are in the front rank of your group.
//Caslus moves to attack giant war dog.
//[HP=583]:status
//Name: Darmius Senru                    Lives/CP:      9/5
//Race: Dwarf Exp: 96269611        Perception:     37
//Class: Warrior Level: 40            Stealth:         0
//Hits:   583/640   Armour Class:  65/18 Thievery:        0
//                                       Traps:           0
//                                       Picklocks:       0
//Strength:  135    Agility: 30          Tracking:        0
//Intellect: 40     Health:  130         Martial Arts:    4
//Willpower: 50     Charm:   40          MagicRes:       57
}