using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm.Session
{
    //represents the macro's we have in use, need to save/load this, make a form etc
    public class Macros
    {
        Dictionary<Keys, string> macros = new Dictionary<Keys, string>();
        public Macros()
        {
            this.macros.Add(Keys.NumPad1, "sw\r\n");
            this.macros.Add(Keys.NumPad2, "s\r\n");
            this.macros.Add(Keys.NumPad3, "se\r\n");
            this.macros.Add(Keys.NumPad4, "w\r\n");
            this.macros.Add(Keys.NumPad5, "l ");
            this.macros.Add(Keys.NumPad6, "e\r\n");
            this.macros.Add(Keys.NumPad7, "nw\r\n");
            this.macros.Add(Keys.NumPad8, "n\r\n");
            this.macros.Add(Keys.NumPad9, "ne\r\n");
            this.macros.Add(Keys.NumPad0, "u\r\n");
            this.macros.Add(Keys.Decimal, "d\r\n");
            this.macros.Add(Keys.Divide, "sea\r\n");
            this.macros.Add(Keys.Add, "sys map\r\n");
        }
      
        internal bool IsMacro(Keys keyChar)
        {
            //throw new NotImplementedException();
            return this.macros.ContainsKey(keyChar);
        }

        internal string GetMacro(Keys keyCode)
        {
            return this.macros[keyCode];
        }
    }
}
