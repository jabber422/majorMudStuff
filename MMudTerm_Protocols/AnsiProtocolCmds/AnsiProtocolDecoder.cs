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
        byte[] lastBuffer;
        object InUse = new object();

        protected override void DecodeBuffer(byte[] buffer)
        {
#if DEBUG_2

            Debug.WriteLine("ENTER: " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    this.GetType().Namespace);
            Debug.Indent();
#endif
            //append the saved buffer if any
            buffer = ConcatBuffers(buffer);
            values.Clear();
            pieces.Clear();

            if (buffer[0] == 0xff)
            {
                //0xff is start of a telnet IAC cmd, ignore it
#if DEBUG_2
                Debug.WriteLine("Recieved telent IAC in ANSI parser, this is bad", this.GetType().Namespace);
#endif
                //ignoring Telnet commands at the front of a buffer
                if ((buffer.Length - 3) > 0)
                {
                    //TODO: handle these one day
                    byte[] buf2 = new byte[buffer.Length - 3];
                    Buffer.BlockCopy(buffer, 3, buf2, 0, buffer.Length - 3);
                    this.DecodeBuffer(buf2);
                }
            }
            else //if (buffer[0] == 0x1b) // 'ESC'
            {
                //take the buffer and convert each iac and string to a TermCmd
                int idx = this.TokenizeAnsiCommandBuffer(buffer);
#if DEBUG_2
                string s = String.Format("Tokenize Completed: BufferIdx= {0}, buffer.len= {1}",
                    idx, buffer.Length);
                Debug.WriteLine(s, this.GetType().Namespace);
#endif
                //save off the rest
                if (idx != buffer.Length && idx != -1)
                    SaveBuffer(buffer, idx);
            }
#if DEBUG_2
        Debug.Unindent();
#endif
            lastBuffer = new byte[buffer.Length];
            Buffer.BlockCopy(buffer, 0, lastBuffer, 0, buffer.Length);
        }

        //internal class AnsiDecoder {
        List<TermCmd> TermCmdList = new List<TermCmd>();

        public AnsiProtocolDecoder(ConnObj connObj) : base(connObj)
        {
        }

        /// <summary>
        /// Process a message from the server and turn it into TermCommands
        /// </summary>
        /// <param name="buffer">message buffer from server</param>
        internal int TokenizeAnsiCommandBuffer(byte[] buffer)
        {
            //we've found a 0x1b, mark the index and handle as command
            //buffer[0] is a 0x1b, ot should be
            int IDX = 0;
            bool InCommandStruct = false;

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
                        IDX = idx;
                    }
                }

                IDX = idx;
            }

            //no idea why i did this this way, but it had issues
            PiecesToValues();
            if (values.Count > 0)
            {
                CreateTextCmd();
            }
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
                    //case ANSI_ESC.Graphics: TermCmdsQueue.Enqueue(new AnsiGraphicsCmd(values));
                    //    break;
                    //case ANSI_ESC.CursorBkwd: TermCmdsQueue.Enqueue(new AnsiCursorBkwdCmd(values[0]));
                    //    break;
                    //case ANSI_ESC.EraseDisplay: TermCmdsQueue.Enqueue(new AnsiEraseDisplayCmd());
                    //    break;
                    //case ANSI_ESC.EraseLine: TermCmdsQueue.Enqueue(new AnsiEraseLineCmd());
                    //    break;
                    //case ANSI_ESC.CursorPosition:
                    //case ANSI_ESC.CursorPositionf: TermCmdsQueue.Enqueue(new AnsiCursorPosition(values));
                    //    break;
                    //case ANSI_ESC.CursorUp: TermCmdsQueue.Enqueue(new AnsiCursorUpCmd(values[0]));
                    //    break;
                    //case ANSI_ESC.CursorFwd: TermCmdsQueue.Enqueue(new AnsiCursorFwdCmd(values[0]));
                    //    break;
                    //case ANSI_ESC.noIdea: break;
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

    ////Take a stream of TermCmds and drive a state engine
    //public class GameDecoder 
    //{
    //    public GameDecoder(ConnObj connObj) : base(connObj)
    //    {
    //    }

    //    public override List<ProtocolCommandLine> GetCommandLines()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    protected override void DecodeBuffer(byte[] buffer)
    //    {
            
            

    //    }
    //}
}
