using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System.Drawing;

namespace MMudTerm.Terminal
{
    /// <summary>
    /// This is a circular buffer of TermChar arrays
    /// head is the top line, tail is the bottom line
    /// </summary>
    public class CircularScreenBuffer : IAnsiProtocolCmds, IDisposable
    {
        int _maxRow, _maxCol;
        CircularBufferCounter _tail, _head;
        TermChar[,] buffer;
        TermCurrentGraphic currentGraphic;
        TermCarat carat;
        Encoding encoder;

        const byte caratChar = 0xB1;
        byte[] carat_b = new byte[] { caratChar };

        #region accesors
        public TermCarat Carat
        { get { return this.carat; } }

        public int Head
        { get { return this._head.Value; } }

        public int Tail
        { get { return this._tail.Value; } }

        public int Count
        {
            get
            {
                if (this._head.Value > this._tail.Value)
                {
                    return (this._maxRow - this._head.Value) + this._tail.Value;
                }
                return this._tail.Value - this._head.Value;
            }
        }

        public int MaxRowCount
        { get { return this._maxRow; } }

        public int MaxColCount
        { get { return this._maxCol; } }
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="maxRow">max rows to display</param>
        /// <param name="maxCol">max cols to display</param>
        public CircularScreenBuffer(int maxRow, int maxCol)
        {
            this._maxRow = maxRow;
            this._maxCol = maxCol;
            //windows XP version of this CP is wrong!  find the right one
            encoder = Encoding.GetEncoding(437);
            this.Clear();
        }

        /// <summary>
        /// Resets the grid to default values
        /// </summary>
        public void Clear()
        {
            this.buffer = new TermChar[this._maxRow, this._maxCol];
            InitBuffer();
            this._tail = new CircularBufferCounter(this._maxRow, this._maxRow - 1);
            this._head = new CircularBufferCounter(this._maxRow, 0);

            currentGraphic = new TermCurrentGraphic((int)ANSI_COLOR.White, (int)ANSI_COLOR.Black_B);

            this.carat = new TermCarat(0, 0);
            this.carat.Char = encoder.GetString(carat_b);
            this.carat.AnsiGraphic = currentGraphic.CurrentGraphicCfg;
        }

        //sets all elements to default TermChar's
        private void InitBuffer()
        {
            for (int i = 0; i < this._maxRow; ++i)
            {
                for (int j = 0; j < this._maxCol; ++j)
                {
                    this.buffer[i, j] = new TermChar();
                }
            }
        }

        /// <summary>
        /// Gets a Value from the row/colo
        /// </summary>
        /// <param name="row">The presented row, we handle offseting the values</param>
        /// <param name="col">The column</param>
        /// <returns>TermChar</returns>
        public TermChar GetValue(int row, int col)
        {
            //clone the head counter
            CircularBufferCounter temp = (CircularBufferCounter)this._head.Clone();
            //set the row offset, let it handle wrapping
            temp += row;
            return this.buffer[temp.Value, col];
        }

        //set the termchar at the current position
        private void SetValue(TermChar termChar)
        {
            CircularBufferCounter temp = this._head;
            try
            {

                temp += this.carat.Row;
                this.buffer[temp.Value, this.carat.Col] = termChar;
            }
            catch (Exception ex)
            {
                //should not get here unless something if wrong with the CB counters
            }
        }

        //takes a character and creates a TermChar
        private TermChar CreateTermChar(char c)
        {
            TermChar tc = new TermChar();
            tc.Char = c.ToString();
            tc.AnsiGraphic = this.currentGraphic.CurrentGraphicCfg;
            return tc;
        }

        #region IAnsiProtocolCmds Members

        public void DoEraseDisplay()
        {
            this.Clear();
        }

        public void DoEraseLine()
        {
            CircularBufferCounter temp = this._head;
            temp += this.carat.Row;
            for (int i = this.carat.Col; i < this._maxCol; ++i)
            {
                this.buffer[temp.Value, i].Clear();
            }
        }

        public void DoCursorBkwd(int cols)
        {
            if (cols < this.carat.Col)
            {
                this.carat.Bkwd(cols);
            }
            else
            {
                this.carat.SetCol(0);
            }
        }

        public void DoCursorFwd(int cols)
        {
            if (cols < (this._maxCol - this.carat.Col))
            {
                this.carat.Fwd(cols);
            }
            else
            {
                this.DoCarrigeReturn();
                this.DoNewLine();
            }
        }

        public void DoCursorUp(int rows)
        {
            this.carat.Up(rows);
        }

        public void DoCursorDown(int rows)
        {
            if (rows >= (this._maxRow - this.carat.Row) + this._head.Value)
            {
                this.carat.Down(rows);
            }
            else
            {
                this.carat.SetRow(this._maxRow);
            }
        }

        public void SetCurGraphics(params int[] values)
        {
            this.currentGraphic.Change(values);
        }

        public void SetCursorPosition(int row, int col)
        {
            this.carat.SetRow(row);
            this.carat.SetCol(col);
        }
        #endregion

        #region ITermProtocolCmds Members
        public void DoCarrigeReturn()
        {
            this.carat.SetCol(0);
        }

        public void DoNewLine()
        {
            //check the total count of lines, is it at our max value?
            if (this.carat.Row == this._maxRow - 1)
            {
                this._head += 1; //move the head down
                this._tail += 1; //move the tail down
                DoEraseLine(); //clear the new line in case we have stale data;
            }
            else
            {
                this.carat.IncRow();
            }
        }

        public void DoBackSpace()
        {
            this.carat.Bkwd(1);
        }

        private void AddChar(char c)
        {
            if (c == '\b')
            {
                this.DoCursorBkwd(1);
            }
            else if (c == '\a')
            { } //ignore bells
            else
            {
                SetValue(CreateTermChar(c));
                this.DoCursorFwd(1);
            }
        }

        public void AddCharString(byte[] str)
        {
            string text = Encoding.GetEncoding(437).GetString(str);
            
            for (int i = 0; i < text.Length; ++i)
            {
                this.AddChar(text[i]);
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this._tail = null;
            this._head = null;
            this.buffer = null;
            this.currentGraphic = null;
            this.carat = null;
            this.encoder = null;
        }
        #endregion

        private class CircularBufferCounter : ICloneable
        {
            int _val, _max;
            public int Value
            { get { return this._val; } }

            public int Max
            { get { return this._max; } }

            internal CircularBufferCounter(int max, int value)
            {
                this._max = max;
                this._val = value;
            }

            internal void Resize(int newMax)
            {
                this._max = newMax;
            }

            public static CircularBufferCounter operator +(CircularBufferCounter a, int b)
            {
                int headPlusReq = a.Value + b;
                int diff = headPlusReq - a._max;

                int newVal = (headPlusReq >= a._max) ? diff : headPlusReq;
                return new CircularBufferCounter(a._max, newVal);
            }

            internal int Next()
            {
                return ((this._val + 1) > this._max) ? 0 : (this._val + 1);
            }

            #region ICloneable Members
            public object Clone()
            {
                return new CircularBufferCounter(this._max, this._val);
            }
            #endregion
        }
    }
}
