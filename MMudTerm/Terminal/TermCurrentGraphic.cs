namespace MMudTerm.Terminal
{
    /// <summary>
    /// Contains the current graphic settings for the carat
    /// When a character is "printed" these colors are applied
    /// </summary>
    internal class TermCurrentGraphic
    {
        int def_fColor, def_bColor, curFcolor, curBcolor;
        const byte BRIGHT = 0x80;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="defaultFcolor">default foreground color</param>
        /// <param name="defaultBcolor">default background color</param>
        internal TermCurrentGraphic(int defaultFcolor, int defaultBcolor)
        {
            this.curFcolor = this.def_fColor = defaultFcolor;
            this.curBcolor = this.def_bColor = defaultBcolor;
        }

        /// <summary>
        /// Change the graphic settings
        /// </summary>
        /// <param name="values">an arry of ANSI SGR values</param>
        internal void Change(params int[] values)
        {
            byte bright = 0;
            foreach (int i in values)
            {
                if (i == 0)
                {
                    //clear the settings
                    //interpetation of cmd 0 sucks
                    //this seems to work fine for mmud
                    this.curFcolor = this.def_fColor;
                    this.curBcolor = this.def_bColor;
                }
                else if (i == 1)
                {
                    bright = BRIGHT;
                }
                else if (i >= 30 && i <= 37)
                {
                    if (((this.curFcolor >> 7) == 1 ? true : false))
                        this.curFcolor = i | BRIGHT;
                    else
                        this.curFcolor = i | bright;
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
                    this.curBcolor = i;
#endif
                }
                else
                {

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

        /// <summary>
        /// Returns a deep copy of the CurrentGraphic settings
        /// </summary>
        internal TermAnsiGraphic CurrentGraphicCfg
        {
            get
            {
                TermAnsiGraphic temp = new TermAnsiGraphic();
                temp.Fcolor = this.curFcolor;
                temp.Bcolor = this.curBcolor;
                return temp;
            }
        }
    }
}
