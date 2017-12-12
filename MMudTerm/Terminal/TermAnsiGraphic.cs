using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMudTerm.Terminal
{
    public class TermAnsiGraphic : ICloneable
    {
        int fcolor;
        int bcolor;

        public TermAnsiGraphic()
        {
            Init();
        }

        public int Fcolor
        {
            get { return this.fcolor; }
            internal set { this.fcolor = value; }
        }

        public int Bcolor
        {
            get { return this.bcolor; }
            internal set { this.bcolor = value; }
        }

        internal void Clear()
        {
            Init();
        }

        private void Init()
        {
            //fcolor = (int)ANSI_COLOR.White;
           // bcolor = (int)ANSI_COLOR.Black_B;
        }

        #region ICloneable Members

        public object Clone()
        {
            TermAnsiGraphic rtnVal = new TermAnsiGraphic();
            rtnVal.fcolor = this.fcolor;
            rtnVal.bcolor = this.bcolor;
            return rtnVal;
        }

        #endregion
    }
}
