using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using MMudTerm_Protocols;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System.Text.RegularExpressions;

namespace MMudTerm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
//#if NO_TEST
#if DEBUG
            Debug.AutoFlush = true;
            ConsoleTraceListener ctl = new ConsoleTraceListener();
            TraceListener tl = new TextWriterTraceListener(Console.Out);
            TraceListener tl_2 = new TextWriterTraceListener("AppDebugTrace.trc");
            Debug.Listeners.Add(ctl);
            //Debug.Listeners.Add(tl);
            Debug.Listeners.Add(tl_2);

#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MMudTerm term = null;
            
            try
            {
                term = new MMudTerm();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message +"\r\n"+ ex.Source +"\r\n" + ex.StackTrace, "APP");
            }
            
            try
            {
                Application.Run(term);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + "\r\n" + ex.Source + "\r\n" + ex.StackTrace, "APP");
            }
//#endif


            string cur = "You shoot a bolt at large mummy for 27 damage!";
            
            Regex combatHit = new Regex(@"You [\w_]+ at [\w_]+ for [1-9]+ damage!");
            
            bool ans = combatHit.IsMatch(cur);

            //cur = "You shoot a bolt at";
            combatHit = new Regex(@"You (?<ACTION>[A-Za-z ]+) at (?<TARGET>[A-Za-z ]+) for (?<DMG>[0-9]+) damage!");
            bool ans2 = combatHit.IsMatch(cur);

        }
    }
}
