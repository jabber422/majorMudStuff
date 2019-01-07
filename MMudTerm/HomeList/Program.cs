using MMudObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            Trace.AutoFlush = true;
            //ConsoleTraceListener ctl = new ConsoleTraceListener();
            TraceListener tl = new TextWriterTraceListener(Console.Out);

            string logFile = "MMudTrace.log";
            if (File.Exists(logFile))
            {
                File.Copy(logFile, "MMudTrace_previous.log", true);
                File.Delete(logFile);
            }

            TraceListener file2 = new TextWriterTraceListener(logFile);

            Trace.Listeners.Add(tl);
            //Trace.Listeners.Add(file);
            Trace.Listeners.Add(file2);
            //Debug.Listeners.Add(ctl);
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }catch(Exception ex)
            {
                Log.Tag("Main", "Caught exception in Application.Run()");
                Log.Tag("Main", ex.Message + @"\r\n\");
                Log.Tag("Main", ex.StackTrace);
            }
        }

        static string GetLogFileName()
        {
            string logsFolder = @"c:\sandbox\logs\homelist";
            Directory.CreateDirectory(logsFolder);

            string[] oldLogs = Directory.GetFiles(logsFolder, "homelist*.log");

            int fileIndex = oldLogs.Length + 1;
            if (fileIndex > 10)
            {
                FileInfo oldFile = null;
                foreach (string s in oldLogs)
                {
                    FileInfo fi = new FileInfo(s);
                    if (oldFile == null)
                    {
                        oldFile = fi;
                    }
                    else
                    {
                        if (oldFile.LastWriteTime > fi.LastWriteTime)
                        {
                            oldFile = fi;
                        }
                    }

                }
                File.Delete(oldFile.FullName);
            }

            return "homelist_" + DateTime.Now.ToString("dd_hh_mm");
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
