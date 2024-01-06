using System;

namespace MMudTerm_Protocols.AnsiProtocolCmds
{
    public class AnsiGraphicObj : ICloneable
    {
        int fcolor, bcolor;
        int def_fColor, def_bColor;
        const byte BRIGHT = 0x80;

        public AnsiGraphicObj(int defaultFcolor, int defaultBcolor)
        {
            this.fcolor = this.def_fColor = defaultFcolor;
            this.bcolor = this.def_bColor = defaultBcolor;
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
            //Init();
        }

        /// <summary>
        /// Change the graphic settings
        /// </summary>
        /// <param name="values">an arry of ANSI SGR values</param>
        public void Change(params int[] values)
        {
            byte bright = 0;
            foreach (int i in values)
            {
                if (i == 0)
                {
                    //clear the settings
                    //interpetation of cmd 0 sucks
                    //this seems to work fine for mmud
                    this.fcolor = this.def_fColor;
                    this.bcolor = this.def_bColor;
                }
                else if (i == 1)
                {
                    bright = BRIGHT;
                }
                else if (i >= 30 && i <= 37)
                {
                    if (((this.fcolor >> 7) == 1 ? true : false))
                        this.fcolor = i | BRIGHT;
                    else
                        this.fcolor = i | bright;
                }
                else if (i >= 40 && i <= 47)
                {
                    //gmud doesn't seem to use bright backgrounds, they just cause issues
#if AllowBrightBackground
                    if (isBright(this.curBcolor))
                        this.curBcolor = i | BRIGHT;
                    else
                        this.curBcolor = i | bright;
#else
                    this.bcolor = i;
#endif
                }
            }
        }

        //checks if the color is flagged as bright
        private bool isBright(int color)
        {
            return (color >> 7) == 1 ? true : false;
        }

        //given a value will return the base color, regarless of bright bit being set
        private int GetBaseColor(int color)
        {
            if (isBright(color))
            {
                return (color ^ BRIGHT);
            }
            return color;

        }

        #region ICloneable Members

        public object Clone()
        {
            AnsiGraphicObj rtnVal = new AnsiGraphicObj(def_fColor, def_bColor);
            rtnVal.fcolor = this.fcolor;
            rtnVal.bcolor = this.bcolor;
            return rtnVal;
        }

        #endregion
    }
}
