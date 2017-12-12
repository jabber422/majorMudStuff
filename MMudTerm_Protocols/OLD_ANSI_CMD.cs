using System;
using System.Collections.Generic;
using System.Text;

namespace AnsiProtocol
{
    public static class TermCommandFactory
    {
        static Queue<TermCmd> commands = new Queue<TermCmd>();
        static Stack<byte[]> values = new Stack<byte[]>();
        //telnet commands are peeled before this layer
        //since we are using linemode we can assume that the starting byte is either
        //an echo(~0x1b), an ANSI IAC (0x1b)
        public static Queue<TermCmd> CreateCmds(byte[] buffer)
        {
            lock (commands)
            {
                commands.Clear();
                values.Clear();
                switch (buffer[0])
                {
                    case 0x1b: TokenizeAnsiCommandBuffer(buffer);
                        break;
                    //this WILL be handled at a lower level
                    case 0xff: Console.WriteLine("Recieved telent IAC, this is bad");
                        break;
                    default: HandleAsEcho(buffer);
                        break;
                }
            }
            return commands;
        }

        private static void HandleAsEcho(byte[] buffer)
        {
            commands.Enqueue(new TermEchoCharCmd(buffer));
        }

        /// <summary>
        /// Process a message from the server and turn it into TermCommands
        /// </summary>
        /// <param name="buffer">message buffer from server</param>
        private static void TokenizeAnsiCommandBuffer(byte[] buffer)
        {
            //when we find a point of interest this points 
            //to the index + 1 so we can work around it
            int ansiValuePtr = 0;
            //we've found a 0x1b, we need to start processing as a command and not string data
            bool opened = false;

            for (int idx = 1; idx < buffer.Length; ++idx)
            {
                //Console.WriteLine("Value = {0:X}", buffer[idx]);
                if (opened)
                {
                    switch (buffer[idx])
                    {
                        //found the ], start of cmd
                        case 0x5b:
                            Console.WriteLine("found ]");
                            ansiValuePtr = idx + 1;
                            break;
                        //found a ;, some cmds have multipule values
                        case 0x3b:
                            Console.WriteLine("found ;");
                            ansiValuePtr = GrabValue(buffer, ansiValuePtr, idx);
                            break;
                        //check for an end char
                        default:
                            if (EndChar(buffer[idx]))
                            {
                                Console.WriteLine("found EndChar: {0:X}", buffer[idx]);
                                ansiValuePtr = GrabValue(buffer, ansiValuePtr, idx);
                                CreateCommand((ANSI_CMD)buffer[idx], buffer, ansiValuePtr, idx);
                                opened = false;
                            }
                            break;
                        //if we get to here then we have a 2+ digit value, keep processing
                    }
                }
                else
                {
                    //signals the start of an ansi esc command
                    if (buffer[idx] == 0x1b)
                    {
                        Console.WriteLine("found, term: {0:X}", buffer[idx]);
                        if (idx > ansiValuePtr && ansiValuePtr != 0)
                        {
                            //grab string data nested between commands
                            CreateCommand(ANSI_CMD.DATA, buffer, ansiValuePtr, idx);
                        }
                        opened = true;
                    }
                    //we found a \n, there better be a \r before it or something is wrong
                    else if (buffer[idx] == '\n')
                    {
                        Console.WriteLine("found, term: LF");
                        if (buffer[idx - 1] == '\r')
                        {
                            Console.WriteLine("found, term: CR (before the LF)");
                            //must idx-1 we only want the string
                            CreateCommand(ANSI_CMD.DATA, buffer, ansiValuePtr, idx-1);
                            //now add a newline cmd to the end
                            CreateCommand(ANSI_CMD.NEWLINE, buffer, ansiValuePtr, idx);
                            ansiValuePtr = idx+1;  //set our point of interest after the \n incase a second lines is in packet
                        }
                        else
                        {
                            //WTF???  error?
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Grabs an ansi command value from the buffer and pushes it onto the stack
        /// the values are colors, col, rows etc...
        /// </summary>
        /// <param name="buffer">the message buffer</param>
        /// <param name="ansiValuePtr">index of the start of the value in the buffer</param>
        /// <param name="i">current idx in the buffer</param>
        /// <returns>returns the index to the next posible value, idx+1</returns>
        private static int GrabValue(byte[] buffer, int ansiValuePtr, int idx)
        {
            int size = idx - ansiValuePtr;
            if (size > 0)
            {
                byte[] val = new byte[size];
                Buffer.BlockCopy(buffer, ansiValuePtr, val, 0, size);
                values.Push(val);
            }
            return idx + 1;
        }

        /// <summary>
        /// Creates a concrete TermCmd from the cmd supplied
        /// adds it to the command queue
        /// </summary>
        /// <param name="cmd">The cmd to make</param>
        /// <param name="buffer">the msg buffer</param>
        /// <param name="startIdx">the start idx of extra data after the command</param>
        /// <param name="endIdx">the end idx, either end of string or the start of the next cmd</param>
        private static void CreateCommand(ANSI_CMD cmd, byte[] buffer, int startIdx, int endIdx)
        {
            switch (cmd)
            {
                case ANSI_CMD.DATA: commands.Enqueue(new TermStringDataCmd(buffer, startIdx, endIdx));
                    break;
                case ANSI_CMD.NEWLINE: commands.Enqueue(new TermNewLineCmd());
                    break;
                case ANSI_CMD.Graphics: commands.Enqueue(new TermGraphicsCmd(values));
                    break;
                case ANSI_CMD.CursorBkwd: commands.Enqueue(new TermCursorBkwdCmd(values.Pop()));
                    break;
                case ANSI_CMD.EraseLine: commands.Enqueue(new TermEraseLineCmd());
                    break;
                default: Console.WriteLine("NYI -- {0}", cmd.ToString());
                    break;
            }
        }

        /// <summary>
        /// All Ansi Escape sequence command terminators
        /// </summary>
        /// <param name="p">the byte to test</param>
        /// <returns>true for a command false for not</returns>
        private static bool EndChar(byte p)
        {
            switch ((ANSI_ESC)p)
            {
                case ANSI_ESC.CursorPosition:
                case ANSI_ESC.CursorPositionf:
                case ANSI_ESC.CursorUp:
                case ANSI_ESC.CursorDown:
                case ANSI_ESC.CursorFwd:
                case ANSI_ESC.CursorBkwd:
                case ANSI_ESC.CursorSave:
                case ANSI_ESC.CursorRestore:
                case ANSI_ESC.EraseDisplay:
                case ANSI_ESC.EraseLine:
                case ANSI_ESC.Graphics:
                case ANSI_ESC.Mode:
                case ANSI_ESC.ResetMode:
                case ANSI_ESC.KeyboardString:
                    return true;
                default: return false;
            }        
        }
    }

    

    
    public class TermChar
    {
        internal char c;
        public TermChar()
        {
            c = '\0';
        }
        
        public TermChar(char c)
        {
        }

        public TermChar(char c, Color fColor)
        {
        }

        public TermChar(char c, Color fColor, Color bColor)
        {
        }

        public TermChar(char c, Color fColor, Color bColor, Attribute attr)
        {
        }
    }

    

    public class TermCarat
    {
        int x, y;
    }

    

    

    

    

    

    

    

    
}
