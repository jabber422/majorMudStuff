using MMudTerm_Protocols.AnsiProtocolCmds;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using MMudObjects;

namespace MMudTerm_Protocols
{
    internal abstract class DecoderState
    {
        public abstract DecoderState DoWork(ProtocolDecoderV2 decoder, Queue<byte> b);
    }
    internal abstract class AnsiDecoderState : DecoderState
    {
        public abstract AnsiDecoderState DoWork(AnsiProtocolDecoderV2 decoder, Queue<byte> b);

        public override DecoderState DoWork(ProtocolDecoderV2 decoder, Queue<byte> b)
        {
            return this.DoWork(decoder, b);
        }
    }

    internal class AnsiDecoderState_LookingForIAC : AnsiDecoderState
    {
        private List<byte> _buffer;

        public bool HasData { get { return this._buffer.Count > 0 ? true : false; } }

        public AnsiDecoderState_LookingForIAC()
        {
            this._buffer = new List<byte>();
        }
        //in this state we are looking for the start of an IAC
        public override AnsiDecoderState DoWork(AnsiProtocolDecoderV2 decoder, Queue<byte> queue)
        {
            byte b = queue.Dequeue();

            switch ((ANSI_ESC)b)
            {
                case ANSI_ESC.Esc:
                    FlushBufferToStringCmd(decoder);
                    return new AnsiDecoderState_FoundIAC();
                case ANSI_ESC.Telnet:
                    FlushBufferToStringCmd(decoder);
                    return new AnsiDecoderState_FoundTelnet();
                case ANSI_ESC.NewLine:
                    FlushBufferToStringCmd(decoder);
                    decoder.AddTermCmd(new TermNewLineCmd());
                    return this;
                case ANSI_ESC.CarrigeReturn:
                    FlushBufferToStringCmd(decoder);
                    decoder.AddTermCmd(new TermCarrigeReturnCmd());
                    return this;
            }

            this._buffer.Add(b);
            return this;
        }

        private void FlushBufferToStringCmd(AnsiProtocolDecoderV2 decoder)
        {
            if (this._buffer.Count > 0)
            {
                decoder.AddTermCmd(new TermStringDataCmd(this._buffer));
                this._buffer.Clear();
            }
        }

        //Flush whatever is in the buffer to a text string cmd
        public TermCmd GetTermCmd()
        {
            TermCmd cmd = new TermStringDataCmd(this._buffer);
            this._buffer.Clear();
            return cmd;
        }
    }

    internal class AnsiDecoderState_FoundIAC : AnsiDecoderState
    {
        List<byte> iacCmdBuffer;
        public AnsiDecoderState_FoundIAC()
        {
            iacCmdBuffer = new List<byte>();
        }

        public override AnsiDecoderState DoWork(AnsiProtocolDecoderV2 decoder, Queue<byte> queue)
        {
            //we are at 1+n bytes after an IAC cmd
            byte b = queue.Dequeue();
            if (b == 0x5b) // '['
            {
                //the 2nd part of the IAC, ignore it, stay in this state
                return this;
            }

            if (EndChar(b))
            {
                if (b == 'm')
                { }
                
                TermCmd cmd = CreateCommand((ANSI_ESC)b, iacCmdBuffer);
                iacCmdBuffer.Clear();
                if (cmd != null)
                {
                    decoder.AddTermCmd(cmd);
                }
                else
                {
                    Log.Warn("Got a null TermCmd from CreateCommand, there is one value that is unsupported and ignored - ignoring it");
                }
                return new AnsiDecoderState_LookingForIAC();
            }

            this.iacCmdBuffer.Add(b);
            return this;
        }

        //IAC cmd has to end with one of these values
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

        private TermCmd CreateCommand(ANSI_ESC cmd, List<byte> values)
        {
            switch (cmd)
            {
                case ANSI_ESC.Graphics:
                    return new AnsiGraphicsCmd(values);
                case ANSI_ESC.CursorBkwd:
                    return new AnsiCursorBkwdCmd(values);
                case ANSI_ESC.EraseDisplay:
                    return new AnsiEraseDisplayCmd();
                case ANSI_ESC.EraseLine:
                    return new AnsiEraseLineCmd();
                case ANSI_ESC.CursorPosition:
                case ANSI_ESC.CursorPositionf:
                    return new AnsiCursorPosition(values);
                case ANSI_ESC.CursorUp:
                    return new AnsiCursorUpCmd(values);
                case ANSI_ESC.CursorFwd:
                    return new AnsiCursorFwdCmd(values);
                case ANSI_ESC.noIdea: 
                case ANSI_ESC.KeyboardString:
                    Log.Warn("NYI -- " + cmd.ToString(), this.GetType().FullName);
                    return null;
                default:
                    throw new Exception("AnsiDecoderState.CreateCmd - invalid cmd! -> " + cmd.ToString());

            }
        }
    }

    internal class AnsiDecoderState_FoundTelnet : AnsiDecoderState
    {
        private AnsiProtocolDecoderV2 AnsiProtocolDecoderV2;

        public AnsiDecoderState_FoundTelnet()
        {
        }

        public override AnsiDecoderState DoWork(AnsiProtocolDecoderV2 decoder, Queue<byte> queue)
        {
            throw new System.NotImplementedException();
        }
    }
}