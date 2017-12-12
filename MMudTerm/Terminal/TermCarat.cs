using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MMudTerm.Terminal
{
    public class TermCarat : TermChar
    {
        Point pt;
        public int Row
        { get { return pt.Y; } }

        public int Col
        { get { return pt.X; } }

        public TermCarat(int row, int col)
        {
            pt = new Point(col, row);
        }

        public void SetCursorLoc(int row, int col)
        {
            pt = new Point(col, row);
        }

        internal void SetRow(int row)
        {
            pt.Y = row;
        }

        internal void SetCol(int col)
        {
            pt.X = col;
        }

        internal void IncRow()
        {
            pt.Y++;
        }

        internal void Bkwd(int p)
        {
            pt.X -= p;
        }

        internal void Fwd(int p)
        {
            pt.X += p;
        }

        internal void Up(int p)
        {
            pt.Y -= p;
        }

        internal void Down(int p)
        {
            pt.Y += p;
        }
    }
}
