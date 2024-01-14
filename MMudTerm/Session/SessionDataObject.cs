using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MMudTerm.Session
{
    /// <summary>
    /// This is the master config file(in memory) for a MMudTerm Session.
    /// This will be the base class for session data/config.  Use Decorators to extend this with "other" settings.
    /// </summary>
    [Serializable]
    public class SessionDataObject : IDisposable
    {
        public SessionDataObject(SessionConnectionInfo sci)
        {
            this.m_sci = sci;
            InitGraphics();
            InitLogon();
        }

        #region session connection
        SessionConnectionInfo m_sci;
        public SessionConnectionInfo ConnectionInfo { get { return this.m_sci; } }
        #endregion

        #region Ansi Graphics settings
        //TODO: move to a global config not session
        //global settings for color
        Brush black = new SolidBrush(Color.FromArgb(0, 0, 0)); //black, #30
        Brush red = new SolidBrush(Color.FromArgb(128, 0, 0)); //red, #31
        Brush green = new SolidBrush(Color.FromArgb(0, 128, 0)); //green, 32
        Brush yellow = new SolidBrush(Color.FromArgb(128, 128, 0)); //yellow, 33
        Brush blue = new SolidBrush(Color.FromArgb(0, 0, 128)); //blue, 34
        Brush magenta = new SolidBrush(Color.FromArgb(128, 0, 128)); //magenta, 35
        Brush cyan = new SolidBrush(Color.FromArgb(0, 128, 128)); //cyan, 36
        Brush white = new SolidBrush(Color.FromArgb(192, 198, 198)); //white, 37
        Brush b_black = new SolidBrush(Color.FromArgb(128, 128, 128)); //black, #40
        Brush b_red = new SolidBrush(Color.FromArgb(255, 0, 0)); //red, #41
        Brush b_green = new SolidBrush(Color.FromArgb(0, 255, 0)); //green, 42
        Brush b_yellow = new SolidBrush(Color.FromArgb(255, 255, 0)); //yellow, 43
        Brush b_blue = new SolidBrush(Color.FromArgb(0, 0, 255)); //blue, 44
        Brush b_magenta = new SolidBrush(Color.FromArgb(255, 0, 255)); //magenta, 45
        Brush b_cyan = new SolidBrush(Color.FromArgb(0, 255, 255)); //cyan, 46
        Brush b_white = new SolidBrush(Color.FromArgb(255, 255, 255)); //white, 47

        Brush termBrush = Brushes.LawnGreen;

        Dictionary<int, Brush> pallet = new Dictionary<int, Brush>();

        Color BackGroundColor = Color.Black;

        FontFamily font = FontFamily.GenericMonospace;
        int fontSize = 16;
        Font termFont;

        int rows = 40;
        int cols = 80;

        //init
        private void InitGraphics()
        {
            InitPallet();
            termFont = new Font(font, fontSize);
        }

        private void DisposeGraphics()
        {
            foreach (Brush b in pallet.Values) { b.Dispose(); }
            termBrush.Dispose();
            termFont.Dispose();
        }

        private void InitPallet()
        {
            int BRIGHT = 0x80;
            pallet.Add(30, black);
            pallet.Add(31, red);
            pallet.Add(32, green);
            pallet.Add(33, yellow);
            pallet.Add(34, blue);
            pallet.Add(35, magenta);
            pallet.Add(36, cyan);
            pallet.Add(37, white);
            pallet.Add(40, black);
            pallet.Add(41, red);
            pallet.Add(42, green);
            pallet.Add(43, yellow);
            pallet.Add(44, blue);
            pallet.Add(45, magenta);
            pallet.Add(46, cyan);
            pallet.Add(47, white);
            pallet.Add(30 | BRIGHT, b_black);
            pallet.Add(31 | BRIGHT, b_red);
            pallet.Add(32 | BRIGHT, b_green);
            pallet.Add(33 | BRIGHT, b_yellow);
            pallet.Add(34 | BRIGHT, b_blue);
            pallet.Add(35 | BRIGHT, b_magenta);
            pallet.Add(36 | BRIGHT, b_cyan);
            pallet.Add(37 | BRIGHT, b_white);
            pallet.Add(40 | BRIGHT, b_black);
            pallet.Add(41 | BRIGHT, b_red);
            pallet.Add(42 | BRIGHT, b_green);
            pallet.Add(43 | BRIGHT, b_yellow);
            pallet.Add(44 | BRIGHT, b_blue);
            pallet.Add(45 | BRIGHT, b_magenta);
            pallet.Add(46 | BRIGHT, b_cyan);
            pallet.Add(47 | BRIGHT, b_white);
        }

        internal System.Drawing.Font GetTermFont()
        {
            return termFont;
        }

        internal Brush GetTermBrush()
        {
            return termBrush;
        }

        internal Dictionary<int, Brush> GetTermPallet()
        {
            return pallet;
        }

        internal Color GetBackGroundColor()
        {
            return BackGroundColor;
        }

        internal int GetUserSpecifiedRows()
        {
            return rows;
        }

        internal int GetUserSpecifiedCols()
        {
            return cols;
        }
        #endregion

        #region logon key value pairs
        internal Dictionary<Regex, string> GetJumpToInGameSessionsStrings()
        {
            string pattern_tick = @"\[HP=(\d+)(?:/MA=(\d+))?\]:(?: \((\w+)\))?";
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex(pattern_tick), "");
            
            return dict;
        }

        internal Dictionary<Regex, string> GetLogonDataStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex(@"ID\?"), "ID,2,1");
            //dict.Add(new Regex(@"CON\?"), "CON,bbs.classicmud.com,2323");
            dict.Add(new Regex("Otherwise type \"new\":"), @"jabber"+((char)0x0d).ToString());
            dict.Add(new Regex("Enter your password:"), @"zetafoo" + ((char)0x0d).ToString());


            return dict;
        }

        internal Dictionary<Regex, string> GetMenuDataStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex("Main System Menu \\(TOP\\)"), "g" + ((char)0x0d).ToString());
            dict.Add(new Regex("GAMES \\(GAMES\\)"), "m" + ((char)0x0d).ToString());
            return dict;
        }


//         M A J O R  M U D v1.11p-WG3NT (Dec 30 2005 14:20:26)
//{Realm Of Legends}
//*ANSI RECOMMENDED*

//[E] . Enter the Realm
//[H] . Help
//[A] . About the Game
//[C] . Game Counts
//[T] . Topten Adventurers
//[G] . Topten Gangs
//[W] . Who's in the Realm
//[R] . Release Notes
//[I] . Information
//[X] . Exit Game

//[*] . MajorMUD Plus Extensions
        internal Dictionary<Regex, string> GetMajorMudMenuStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex(" M A J O R  M U D v"), "");
            dict.Add(new Regex("{Realm Of Legends}"), "");
            dict.Add(new Regex("\\[E\\] . Enter the Realm"), "");
            dict.Add(new Regex("\\[MAJORMUD\\]:"), "e" + ((char)0x0d).ToString());
            return dict;
        }

        internal Dictionary<Regex, string> GetMajorMudCharCreationStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex("Please choose a race from the following list:"), "");
            dict.Add(new Regex("Please choose your race "), "");
            return dict;
        }

        internal Dictionary<Regex, string> GetMajorMudCharPageStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex("M A J O R  M U D Character Creation"), "");
            return dict;
        }

        internal Dictionary<Regex, string> GetMajorMudEnterGameStrings()
        {
            Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
            dict.Add(new Regex(" M A J O R  M U D v"), "");
            dict.Add(new Regex("{Realm Of Legends}"), "");
            dict.Add(new Regex("[E] . Enter the Realm"), "");
            dict.Add(new Regex("[MAJORMUD]:"), "e" + ((char)0x0d).ToString());
            return dict;
        }

        public bool ProxyEnabled { get; set; }
        public bool LogonEnabled { get; set; }
        public bool EnterGameEnabled { get; set; }
        public bool MummyScriptEnabled { get; set; }

        private void InitLogon()
        {
            this.ProxyEnabled = false;
            this.LogonEnabled = true;
            this.EnterGameEnabled = true;
        }
        #endregion


        public void Dispose()
        {
            DisposeGraphics();
        }









        
    }
}
