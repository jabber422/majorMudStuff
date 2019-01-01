using MMudObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeList
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Log.Tag("foo", "x{0}", "y");
            Debug.AutoFlush = true;
            ConsoleTraceListener ctl = new ConsoleTraceListener();
            TraceListener tl = new TextWriterTraceListener(Console.Out);

            Trace.Listeners.Add(tl);
            //Debug.Listeners.Add(ctl);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public class MyFilter : TraceFilter
    {
        public override bool ShouldTrace(TraceEventCache cache, string source, TraceEventType eventType, int id, string formatOrMessage, object[] args, object data1, object[] data)
        {
            if (source.Contains("GameProcessor")) return true;
            return false;
        }
    }
}
