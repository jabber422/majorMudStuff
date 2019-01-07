using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MMudTerm_Protocols.Engine
{
    internal static class TermCmdDataBlockFactory
    {
        static TermCmdDataBlockAbstractFactory Factory = new TermCmdDataBlockAbstractFactory();
        internal static TermCmdDataBlock Create(List<TermCmd> cmds)
        {
            //if (!((cmds[cmds.Count - 1] is AnsiEraseDisplayCmd) || (cmds[cmds.Count - 1] is AnsiEraseLineCmd)))
            //    throw new Exception("Invalid cmd patterm for a TermCmdDataBlock");

            List<TermCmd> myBlock = new List<TermCmd>();
            ANSI_COLOR attrib = ANSI_COLOR.All_off;
            ANSI_COLOR foreground = ANSI_COLOR.All_off;

            TermStringDataCmd lastStrCmd = null;
            bool colorChanged = false;
            foreach (TermCmd cmd in cmds)
            {
                if (cmd is TermStringDataCmd stringDataCmd)
                {
                    if (lastStrCmd != null)
                    {
                        lastStrCmd.Concat(stringDataCmd);
                    }
                    else
                    {
                        lastStrCmd = stringDataCmd;
                        myBlock.Add(stringDataCmd);
                    }
                    colorChanged = false;
                }
                else if (cmd is AnsiGraphicsCmd colorCmd)
                {
                    lastStrCmd = null;
                    if (attrib != colorCmd.Attribute)
                    {
                        attrib = colorCmd.Attribute;
                        colorChanged = true;
                    }

                    if (foreground != colorCmd.ForeGround)
                    {
                        foreground = colorCmd.ForeGround;
                        colorChanged = true;
                    }

                    myBlock.Add(cmd);
                }
                else if (cmd is TermNewLineCmd nlCmd)
                {
                    if (lastStrCmd != null && colorChanged == false)
                    {
                        lastStrCmd.Concat(nlCmd);
                    }
                }
                else if (cmd is TermCarrigeReturnCmd crCmd)
                {
                    if (lastStrCmd != null && colorChanged == false)
                    {
                        lastStrCmd.Concat(crCmd);
                    }
                }
                else
                {
                    myBlock.Add(cmd);
                    //ignore everything else, for now
                }
            }

            UnknownTermCmdDataBlock block = new UnknownTermCmdDataBlock(myBlock);
            return DetrmineBlockType(block);
        }


        static ANSI_COLOR attrib = ANSI_COLOR.All_off;
        static ANSI_COLOR foreGround = ANSI_COLOR.All_off;

        //assumption is that this will only start with ansi or string cmd.  look for those to determine if this block is a known block
        private static TermCmdDataBlock DetrmineBlockType(UnknownTermCmdDataBlock block)
        {
            int cnt = 0;
            foreach (TermCmd cmd in block.cmds)
            {
                if (cmd is TermStringDataCmd stringDataCmd)
                {
                    string str = stringDataCmd.GetValue();
                    if (MatchAndCaptureTables.Cache2[attrib].ContainsKey(foreGround))
                    {
                        foreach (MatchAndCapture r in MatchAndCaptureTables.Cache2[attrib][foreGround])
                        {
                            if (r.IsMatch(str))
                            {
                                try
                                {
                                    return (TermCmdDataBlock)Activator.CreateInstance(r.type, block);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    throw ex;
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }
                    }
                }
                else if (cmd is AnsiGraphicsCmd colorCmd)
                {
                    if (attrib != colorCmd.Attribute)
                    {
                        attrib = colorCmd.Attribute;
                    }

                    if (foreGround != colorCmd.ForeGround)
                    {
                        foreGround = colorCmd.ForeGround;
                    }
                }
                if (cnt > 1)
                {
                    Log.Warn("DetrmineBlockType is on it's third index??");
                }
                cnt++;
            }

            return block;
        }
    }
}