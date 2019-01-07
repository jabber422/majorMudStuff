using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;

namespace MMudTerm_Protocols.Engine
{
    //buffer term commands into a TermCmdDataBlock, (make this a class? currently list<termcmd>
    //block starts with color/string or CrsrBkwds, i think this will become everything that isn't and end cmd, but i want to test the game design
    //block starts with a clear line/display cmd

    internal abstract class TermCmdDataBlock
    {
        internal List<TermCmd> cmds;

        public TermCmdDataBlock(List<TermCmd> myBlock)
        {
            this.cmds = myBlock;
        }

        internal abstract void UpdateModel(Engine engine);
        internal abstract bool ValidateBlock(TermCmdDataBlock block);
    }

    //everything starts as unknown and either gets matched or stays and unknonwn and is ignored
    internal class UnknownTermCmdDataBlock : TermCmdDataBlock
    {
        public UnknownTermCmdDataBlock(List<TermCmd> myBlock) : base(myBlock)
        {
            cmds = myBlock;
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