using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorState_Idle : GameProcessorState
    {
        internal override GameProcessorState HandleTermCmd(WorkerState_InGame workerState, TermCmd cmd)
        {
            return this.GetNextState(workerState, cmd);
        }

        internal override void HandleTermStringDataCmd(WorkerState_InGame workerState, TermStringDataCmd stringCmd)
        {
            // throw new NotImplementedException());
        }
    }

    static internal class GameProcess_AnsiColorStateMap {

        static internal GameProcessorState[,,] table;
        static internal Dictionary<int, GameProcessorState> Normal;
        static internal Dictionary<int, GameProcessorState> Bold;

        static GameProcess_AnsiColorStateMap()
        {
            table = new GameProcessorState[2, 8, 8];
            table[0, 0, 0] = new GameProcessorState_Black();
            table[0, 1, 1] = new GameProcessorState_Red();
            table[0, 2, 2] = new GameProcessorState_Green();
            table[0, 3, 3] = new GameProcessorState_Yellow();
            table[0, 4, 4] = new GameProcessorState_Blue();
            table[0, 5, 5] = new GameProcessorState_Magenta();
            table[0, 6, 6] = new GameProcessorState_Cyan();
            table[0, 7, 7] = new GameProcessorState_White();

            table[1, 0, 0] = new GameProcessorState_BoldBlack();
            table[1, 1, 1] = new GameProcessorState_BoldRed();
            table[1, 2, 2] = new GameProcessorState_BoldGreen();
            table[1, 3, 3] = new GameProcessorState_BoldYellow();
            table[1, 4, 4] = new GameProcessorState_BoldBlue();
            table[1, 5, 5] = new GameProcessorState_BoldMagenta();
            table[1, 6, 6] = new GameProcessorState_BoldCyan();
            table[1, 7, 7] = new GameProcessorState_BoldWhite();

            Normal = new Dictionary<int, GameProcessorState>();
            Normal.Add(30, new GameProcessorState_Black());
            Normal.Add(31, new GameProcessorState_Red());
            Normal.Add(32, new GameProcessorState_Green());
            Normal.Add(33, new GameProcessorState_Yellow());
            Normal.Add(34, new GameProcessorState_Blue());
            Normal.Add(35, new GameProcessorState_Magenta());
            Normal.Add(36, new GameProcessorState_Cyan());
            Normal.Add(37, new GameProcessorState_White());

            Bold = new Dictionary<int, GameProcessorState>();
            Bold.Add(30, new GameProcessorState_BoldBlack());
            Bold.Add(31, new GameProcessorState_BoldRed());
            Bold.Add(32, new GameProcessorState_BoldGreen());
            Bold.Add(33, new GameProcessorState_BoldYellow());
            Bold.Add(34, new GameProcessorState_BoldBlue());
            Bold.Add(35, new GameProcessorState_BoldMagenta());
            Bold.Add(36, new GameProcessorState_BoldCyan());
            Bold.Add(37, new GameProcessorState_BoldWhite());
        }
    }
}