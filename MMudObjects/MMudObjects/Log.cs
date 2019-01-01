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
        static bool gameprocessorstate = true;

        private static void Msg(string tag, string msg, params string[] args)
        {
            string callingMethod = new StackTrace(1).GetFrame(1).GetMethod().Name;
            //Console.WriteLine(callingMethod);

            if(
                (tag.ToLower() == nameof(enter) && enter) ||
                (tag.ToLower() == nameof(exit) && exit) ||
                (tag.ToLower() == nameof(gameprocessorstate) && gameprocessorstate)
                )
            {
                string s = "";
                try
                {
                    s = callingMethod + ": " + msg;
                    Trace.WriteLine(string.Format(s, args), tag);
                }catch(Exception ex)
                {
                    //wtf is going on here
                }
            }

            //frame.
            //trace[1];

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
    }
}
