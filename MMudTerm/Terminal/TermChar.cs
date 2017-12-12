using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMudTerm_Protocols.AnsiProtocolCmds;

namespace MMudTerm.Terminal
{
    public class TermChar : ICloneable
    {
        string c = "\0";
        TermAnsiGraphic graphic;

        public TermAnsiGraphic AnsiGraphic
        {
            get { return this.graphic; }
            internal set { this.graphic = value; }
        }

        public string Char
        { 
            get { return this.c; }
            internal set { this.c = value; }
        }
        
        internal void Clear()
        {
            this.c = "\0";
            if (this.graphic == null)
                return;
            this.graphic.Clear();
        }
       
        #region ICloneable Members

        public object Clone()
        {
            TermChar rtnVal = new TermChar();
            rtnVal.Char = this.c;
            if (this.graphic != null)
            {
                rtnVal.graphic = (TermAnsiGraphic)this.graphic.Clone();
            }
            return rtnVal;
        }
        #endregion
    }

    
}
