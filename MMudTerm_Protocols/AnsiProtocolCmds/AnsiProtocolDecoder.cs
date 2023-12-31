using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    /// <summary>
    /// This will decode ANSI markup strings and convert them to commands
    /// currently used per instance
    /// </summary>
    public class AnsiProtocolDecoder : TelnetProtocolDecoder
    {
        bool InCommandStruct = false;
        byte[] m_buffer = new byte[4096];
        int offset = 0;
        int count = 0;
        public override Queue<TermCmd> DecodeBuffer(byte[] buffer)
        {
#if DEBUG_2

            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
            Debug.Indent();
#endif
            lock (commandsForTheTerminalScreen)
            {
                //Buffer.BlockCopy(m_buffer, offset, buffer, 0, buffer.Length)
                //append the saved buffer if any
                buffer = ConcatBuffers(buffer);

                if(commandsForTheTerminalScreen.Count > 0) {
                    commandsForTheTerminalScreen.Clear();
                }


                if (buffer[0] == 0xff)
                {
                    while (buffer.Length > 0 && buffer[0] == 0xff)
                    {
                        //0xff is start of a telnet IAC cmd, ignore it
#if DEBUG_2
                    Debug.WriteLine("Recieved telent IAC in ANSI parser, this is bad", this.GetType().Namespace);
#endif
                        //ignoreing Telnet commands at the front of a buffer
                        if (buffer.Length >= 3) 
                        {
                            //TODO: handle these one day
                            byte[] buf1 = new byte[3];
                            Buffer.BlockCopy(buffer, 0, buf1, 0, 3);
                            List<byte[]> list1 = new List<byte[]>();
                            list1.Add(buf1);
                            TermIAC iac = new TermIAC(list1);
                            commandsForTheTerminalScreen.Enqueue(iac);

                            byte[] buf2 = new byte[buffer.Length - 3];
                            Buffer.BlockCopy(buffer, 3, buf2, 0, buffer.Length - 3);
                            buffer = buf2;
                        }
                    }
                }
                else //if (buffer[0] == 0x1b) // 'ESC'
                {
                    
                    int idx = this.TokenizeAnsiCommandBuffer(buffer);

                    if (idx != -1)
                    {
                        
                    }
#if DEBUG_2
                    string s = String.Format("Tokenize Completed: BufferIdx= {0}, buffer.len= {1}",
                        idx, buffer.Length);
                    Debug.WriteLine(s, this.GetType().Namespace);
#endif
                    //if we aren't in the middle of a control command, dump the rest of the buffer as a test string
                    if (!this.InCommandStruct)
                    {
                        CreateTextCmd();
                    }
                    else
                    {

                    }

                }
            }
#if DEBUG_2
            Debug.Unindent();
#endif
            return commandsForTheTerminalScreen;
        }

        /// <summary>
        /// Process a message from the server and turn it into TermCommands
        /// </summary>
        /// <param name="buffer">message buffer from server</param>
        private int TokenizeAnsiCommandBuffer(byte[] buffer)
        {
            int IDX = 0;
            

            for (int idx = 0; idx < buffer.Length; ++idx)
            {
                
                if (InCommandStruct)
                {
                    if (HandledAsCommand(buffer[idx]))
                        InCommandStruct = false;
                }
                else
                {
                    if (HandleAsText(buffer[idx]))
                    {
                        InCommandStruct = true;
                    }
                }

                IDX++;
            }

            //clean up what ever is left over in the buffers
            //CreateTextCmd();
            //this.PostToAi();
            return IDX;
        }

        /// <summary>
        /// Turns the buffers into a term data command
        /// </summary>
        private void CreateTextCmd()
        {
            PiecesToValues();
            if (values.Count > 0)
                CreateCommand(TERM_CMD.DATA);
        }

        /// <summary>
        /// interpets bytes assuming this is text
        /// </summary>
        /// <param name="curByte">the byte to process</param>
        /// <returns>true if a command opener is found, false if not</returns>
        private bool HandleAsText(byte curByte)
        {
            switch (curByte)
            {
                case 0x1b:  // 'ESC'
                    CreateTextCmd();
                    return true;
                case 0x0d:  // 'CR'
                    CreateTextCmd();
                    CreateCommand(TERM_CMD.CR);
                    break;
                case 0x0a:  // 'NL'
                    CreateCommand(TERM_CMD.NL);
                    break;
                default:
                    pieces.Push(curByte);
                    break;
            }
            return false;
        }

        /// <summary>
        /// interpets bytes assuming this is a command
        /// </summary>
        /// <param name="curByte">the byte to process</param>
        /// <returns>true if the command is done, false if not</returns>
        private bool HandledAsCommand(byte curByte)
        {
            if (curByte == 0x5b) // '['
            { }
            else if (curByte == 0x3b) // ';'
            {
                PiecesToValues();
            }
            else if (EndChar(curByte))
            {
                PiecesToValues();
                CreateCommand((ANSI_ESC)curByte);
                return true;
            }
            else
            {
                pieces.Push(curByte);
            }
            return false;
        }

        /// <summary>
        /// Converts a stack into a byte[] segment of the original buffer
        /// </summary>
        private void PiecesToValues()
        {
            if (pieces.Count == 0)
                return;

            byte[] byteValue = new byte[pieces.Count];
            for (int i = pieces.Count; i > 0; --i)
            {
                byteValue[i - 1] = pieces.Pop();
            }
            values.Add(byteValue);
        }

        /// <summary>
        /// Creates a concrete TermCmd from the cmd supplied
        /// adds it to the command queue
        /// </summary>
        /// <param name="cmd">The cmd to make</param>
        /// <param name="buffer">DEL the msg buffer</param>
        /// <param name="startIdx">DEL the start idx of extra data after the command</param>
        /// <param name="endIdx">DEL the end idx, either end of string or the start of the next cmd</param>
        private void CreateCommand(ANSI_ESC cmd)//, byte[] buffer, int startIdx, int endIdx)
        {
            //values[0] not the safest idea, but these commands should never
            //have more than 1 value
            try
            {
                switch (cmd)
                {
                    case ANSI_ESC.Graphics: commandsForTheTerminalScreen.Enqueue(new AnsiGraphicsCmd(values));
                        break;
                    case ANSI_ESC.CursorBkwd: commandsForTheTerminalScreen.Enqueue(new AnsiCursorBkwdCmd(values[0]));
                        break;
                    case ANSI_ESC.EraseDisplay: commandsForTheTerminalScreen.Enqueue(new AnsiEraseDisplayCmd());
                        break;
                    case ANSI_ESC.EraseLine: commandsForTheTerminalScreen.Enqueue(new AnsiEraseLineCmd());
                        break;
                    case ANSI_ESC.CursorPosition:
                    case ANSI_ESC.CursorPositionf: commandsForTheTerminalScreen.Enqueue(new AnsiCursorPosition(values));
                        break;
                    case ANSI_ESC.CursorUp: commandsForTheTerminalScreen.Enqueue(new AnsiCursorUpCmd(values[0]));
                        break;
                    case ANSI_ESC.CursorFwd: commandsForTheTerminalScreen.Enqueue(new AnsiCursorFwdCmd(values[0]));
                        break;
                    case ANSI_ESC.noIdea: break;
                    default:
#if DEBUG
                        Debug.WriteLine("NYI -- " + cmd.ToString(), this.GetType().FullName);
#endif
                        break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
#if DEBUG
                //this shouldn't happen unless the buffers are messed up
                string s = String.Format("CreateCommand() -- ArgOoR Exception for CMD: {0}\r\n" +
                    "                 bad idx used in the values List: VaulesCnt: {1}:", cmd.ToString(), values.Count);
                foreach (byte[] bAry in values)
                {
                    foreach (byte b in bAry)
                    {
                        s += String.Format("{0:X}-", b);
                    }
                    s += "|";

                }
                Debug.WriteLine(s, this.GetType().FullName);
#endif
            }
            values.Clear();
        }

        /// <summary>
        /// All Ansi Escape sequence command terminators
        /// </summary>
        /// <param name="p">the byte to test</param>
        /// <returns>true for a command false for not</returns>
        private bool EndChar(byte p)
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
                case ANSI_ESC.noIdea:
                    return true;
                default: return false;
            }
        }
    }
}
