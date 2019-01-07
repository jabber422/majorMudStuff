using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MMudObjects
{
    public static class Log
    {
        static bool enter = false;
        static bool exit = false;
        static bool gameprocessorstate = false;
        static bool matchandcapture = false;
        static bool block = true;
        static bool debug = false;
        static bool warn = false;
        static bool error = true;
        static bool info = true;
        static bool engine = false;
        static bool workerstate = false;

        private static void Msg(string tag, string msg, params string[] args)
        {
            string callingMethod = new StackTrace(1).GetFrame(1).GetMethod().Name;
            //Console.WriteLine(callingMethod);
            string tagLowerCase = tag.ToLower();
            if(
                (tagLowerCase == nameof(enter) && enter) ||
                (tagLowerCase == nameof(exit) && exit) ||
                (tagLowerCase == nameof(gameprocessorstate) && gameprocessorstate) ||
                (tagLowerCase == nameof(matchandcapture) && matchandcapture) ||
                (tagLowerCase == nameof(block) && block) ||
                (tagLowerCase == nameof(debug) && debug) ||
                (tagLowerCase == nameof(warn) && warn) ||
                (tagLowerCase == nameof(error) && error) ||
                (tagLowerCase == nameof(engine) && engine) ||
                (tagLowerCase == nameof(workerstate) && workerstate) ||
                (tagLowerCase == nameof(info) && info)
                )
            {
                string s = "";
                try
                {
                    s = callingMethod + ": " + msg;
                    Trace.WriteLine(DateTime.Now.ToString("mm:ss.fff") + ": " + string.Format(s, args), string.Format("{0:10}", tag));
                    Trace.Flush();
                }catch(Exception ex)
                {
                    //wtf is going on here
                }
            }
        }

        public static void Enter(string v = "", params string[] args)
        {
            Msg("Enter", v, args);
        }

        public static void Exit(string v = "", params string[] args)
        {
            Msg("Exit", v, args);
        }

        public static void Tag(string s, string v, params string[] args)
        {
            Msg(s, v, args);
        }

        public static void Warn(string v, params string[] args)
        {
            Msg("WARN", v, args);
        }

        public static void Error(string v, params string[] args)
        {
            Msg("ERROR", v, args);
        }

        public static void Debug(string v, params string[] args)
        {
            Msg("Debug", v, args);
        }
    }
}
