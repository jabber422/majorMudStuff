using MMudObjects;
using MMudTerm_Protocols.AnsiProtocolCmds;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    internal static class MyRegex
    {
        internal static Dictionary<Type, List<RegexAndResponse>> Cache;
        internal static Dictionary<Type, List<MatchAndCapture>> Cache2;
        internal static RegexAndResponse HpEquals;
        static MyRegex()
        {
            Cache = new Dictionary<Type, List<RegexAndResponse>>();
            Cache2 = new Dictionary<Type, List<MatchAndCapture>>();

            HpEquals = new RegexAndResponse(@"^\[HP=$", "");

            //build this from a config on logon find->respond values
            RegexAndResponse LogonPrompt = new RegexAndResponse(@"^ID\?$", "ID,2,42");
            RegexAndResponse GameMenu = new RegexAndResponse(@"^\[MAJORMUD\]:?$", "");
            Cache.Add(typeof(WorkerState_Logon), new List<RegexAndResponse>());
            Cache[typeof(WorkerState_Logon)].Add(LogonPrompt);
            Cache[typeof(WorkerState_Logon)].Add(GameMenu);

            RegexAndResponse GameMenuEnter = new RegexAndResponse(@"^\[MAJORMUD\]:?$", "e");
            Cache.Add(typeof(WorkerState_GameMenu), new List<RegexAndResponse>() { GameMenuEnter });
        }
    }

    //public abstract class CaptureItem
    //{
    //    public Type GameProcesorStateType;
    //    public string Pattern;
        

    //    abstract public bool IsMatch(string text);

    //    abstract public List<string> GetChangeMatches();
    //}

    ////given a color state, will match strings only in that state
    ////when a match occurs will execute match handler
    //public class AnsiColorBaseCaptureItem : CaptureItem
    //{
    //    Match m;
    //    public Regex Regex;
    //    string buffer;

    //    public AnsiColorBaseCaptureItem(Type t, string pattern)
    //    {
    //        this.GameProcesorStateType = t;
    //        this.Pattern = pattern;
    //        this.Regex = new Regex(pattern);
    //    }

    //    public override List<string> GetChangeMatches()
    //    {
    //        List<string> result = new List<string>();
    //        foreach (Group g in m.Groups)
    //        {
    //            result.Add(g.Value);
    //        }

    //        return result;
    //    }

    //    public override bool IsMatch(string text)
    //    {
    //        m = this.Regex.Match(text);
    //        return this.m.Success;
    //    }
    //}

    //public class StringBasedCaptureItem : CaptureItem
    //{
    //    private Type type;
    //    private string v;

    //    public StringBasedCaptureItem(Type type, string v)
    //    {
    //        this.type = type;
    //        this.v = v;
    //    }

    //    public override List<string> GetChangeMatches()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsMatch(string text)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    internal static class MatchAndCaptureTables
    {
        internal static Dictionary<ANSI_COLOR, List<MatchAndCapture>> Normal;
        internal static Dictionary<ANSI_COLOR, List<MatchAndCapture>> Bold;
        internal static Dictionary<ANSI_COLOR, Dictionary<ANSI_COLOR, List<MatchAndCapture>>> Cache2;
        static MatchAndCaptureTables()
        {

            //Cache.Add(ANSI_COLOR.Green, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Black, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Blue, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Cyan, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Magenta, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Red, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.White, new List<MatchAndCapture>());
            //Cache.Add(ANSI_COLOR.Yellow, new List<MatchAndCapture>());

            //Cache2 = new Dictionary<ANSI_COLOR, Dictionary<ANSI_COLOR, List<MatchAndCapture>>>();
            //Cache2.Add(ANSI_COLOR.All_off, Cache);
            //Cache2.Add(ANSI_COLOR.Bold, Cache);

            Dictionary<ANSI_COLOR, List<MatchAndCapture>> NormalStrings = new Dictionary<ANSI_COLOR, List<MatchAndCapture>>();
            Dictionary<ANSI_COLOR, List<MatchAndCapture>> BoldStrings = new Dictionary<ANSI_COLOR, List<MatchAndCapture>>();
            Cache2 = new Dictionary<ANSI_COLOR, Dictionary<ANSI_COLOR, List<MatchAndCapture>>>();
            Cache2.Add(ANSI_COLOR.All_off, NormalStrings);
            Cache2.Add(ANSI_COLOR.Bold, BoldStrings);

            #region Green Strings
            NormalStrings.Add(ANSI_COLOR.Green, new List<MatchAndCapture>());

            //Stats
            NormalStrings[ANSI_COLOR.Green].Add(
                new MatchAndCapture(
                    @"^Name: $",
                    typeof(StatsTermCmdDataBlock)
                )
            );
            #endregion


            #region Cyan Strings
            NormalStrings.Add(ANSI_COLOR.Cyan, new List<MatchAndCapture>());
            NormalStrings[ANSI_COLOR.Cyan].Add(
                new MatchAndCapture(
                    @"^\[HP=$",
                    typeof(HealthUpdateCmdDataBlock)
                )
            );
            #endregion

            //Inventory one liner
            //NormalStrings[ANSI_COLOR.Cyan].Add(
            //    new MatchAndCapture(
            //        @"^You notice (.*) here.$",
            //       typeof(ObviousItemsCmdDataBlock)
            //    )
            //);


            //NormalStrings[ANSI_COLOR.Cyan].Add(
            //  new MatchAndCapture(
            //      @"^Obvious exits: (.*)",  //multi line, ends on comma
            //      "Player.Room.ObviousExits",
            //      MatchAndCaptureProp.MatchUntilColorChange & MatchAndCaptureProp.MatchUntilEnd
            //  )
            //);

            ////[HP=
            //NormalStrings[ANSI_COLOR.Cyan].Add(
            //   new MatchAndCapture(@"^\[HP=$", @"^(\d+)$", "Player.Stats.CurHits", MatchAndCaptureProp.MatchNextString)
            //);

            //NormalStrings[ANSI_COLOR.Cyan].Add(
            //   new MatchAndCapture(
            //       @"^\/MA=$", @"^(\d+)$", "Player.Stats.CurMana", MatchAndCaptureProp.MatchNextString)
            //);
            //#endregion





        }
    }



    //internal static class AnsiColorRegexTable
    //{
    //    internal static Dictionary<Type, List<AnsiColorRegex>> Cache;
        

    //    static AnsiColorRegexTable()
    //    {
    //        AnsiColorBaseCaptureItem captureItem1 = new AnsiColorBaseCaptureItem(typeof(GameProcessorState_Green), @"^Name: $");
    //        AnsiColorBaseCaptureItem captureItem2 = new AnsiColorBaseCaptureItem(typeof(GameProcessorState_Cyan), @"(\w+ (\w+)?\s+)");

    //        StringBasedCaptureItem ci1 = new StringBasedCaptureItem(typeof(GameProcessorState_Cyan), "StartsWith=You notice ");
    //        StringBasedCaptureItem ci2 = new StringBasedCaptureItem(typeof(GameProcessorState_Cyan), "EndsWith=here.");
    //        MatchAndCapture ItemsInRoom = new MatchAndCapture(ci1, ci2, "Player.Room.ObvioutItems");


    //        Cache = new Dictionary<Type, List<AnsiColorRegex>>();
    //        List<AnsiColorRegex> ansiColorRegexs = new List<AnsiColorRegex>();
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Name: $", @"(\w+ (\w+)?\s+)", typeof(GameProcessorState_Green), "Player.Stats.Name"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Lives\/CP:\s+$", @"^\s+(\d+)\/(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.LivesCP"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Race: $", @"^(\w+)\s*$", typeof(GameProcessorState_Green), "Player.Stats.Race"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Exp: $", @"^(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Exp"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Perception:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Perception"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Class:\s+$", @"^(\w+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Class"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Level:\s+$", @"^(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Level"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Stealth:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Stealth"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Hits: $", @"^\s+(\d+)\/(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Hits"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Armour Class: $", @"^ (\d+)\/(\d+) $", typeof(GameProcessorState_Green), "Player.Stats.ArmourClass"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Thievery: $", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Thievery"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Traps:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Traps"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^\s+Picklocks:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Picklocks"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Strength:\s+$", @"^\s+(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Strength"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Agility:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Agility"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Tracking:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Tracking"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Intellect:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Intellect"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Health:\s+$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Health"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Martial Arts:+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.MartialArts"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Willpower:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Willpower"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Charm:\s+$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Charm"));
    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^MagicRes:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.MagicRes"));
            

    //        ansiColorRegexs.Add(new AnsiColorRegex(@"^Obvious exits: (.*)$", "", typeof(GameProcessorState_Green), "Player.Room.ObviousExits"));
    //        Cache.Add(typeof(GameProcessorState_Green), ansiColorRegexs);


    //        List<AnsiColorRegex> cyan = new List<AnsiColorRegex>();
 

    //        cyan.Add(new AnsiColorRegex(@"^You notice (.*) here.$", "", typeof(GameProcessorState_Cyan), "Player.Room.ObviousItems.Online"));
    //        cyan.Add(new AnsiColorRegex(@"^You notice (.*)$", "", typeof(GameProcessorState_Cyan), "Player.Room.ObviousItems.Open"));

    //        List<AnsiColorRegex> cyanInventory = new List<AnsiColorRegex>();


    //        cyan.Add(new AnsiColorRegex(@"^(.*)(here.)?$", "", typeof(GameProcessorState_Cyan), "Player.Room.ObviousItems.Close"));

    //        Cache.Add(typeof(GameProcessorState_Cyan), cyan);

    //        List<AnsiColorRegex> boldCyan = new List<AnsiColorRegex>();
    //        Cache.Add(typeof(GameProcessorState_BoldCyan), boldCyan);

    //        //csv
            
    //    }
    //}

    //0 32 0
    //represent a regex that is checked from the corresponding state that matches the bold, foreground, background
    //internal class AnsiColorRegex
    //{
    //    string keyPattern;
    //    string valuePattern;
    //    internal string KeyPattern { get { return keyPattern; } set { if (sourceKey == null) sourceKey = new Regex(value); keyPattern = value; } }
    //    internal string ValuePattern { get { return valuePattern; } set { if (sourceValue == null) sourceValue = new Regex(value); valuePattern = value; } }
    //    Regex sourceKey;
    //    Regex sourceValue;
    //    Type GameProcessorState_Type;
    //    string TargetProperty;

    //    public string Block;
    //    public bool IsInlineMatch = false;

    //    public bool IsIgnored { get; set; }

    //    internal AnsiColorRegex(string srcKeyPattern, string srcValuePattern, Type t, string targetProperty, bool ignore = false)
    //    {
    //        this.KeyPattern = srcKeyPattern;
    //        this.ValuePattern = srcValuePattern;

    //        if (this.ValuePattern == "")
    //        {
    //            this.IsInlineMatch = true;
    //        }


    //        this.GameProcessorState_Type = t;
    //        this.TargetProperty = targetProperty;
    //        this.IsIgnored = ignore;
    //    }

    //    internal bool IsKeyMatch(string stringCmd)
    //    {
    //        return this.sourceKey.IsMatch(stringCmd);
    //    }

    //    internal bool IsValueMatch(string stringCmd)
    //    {
    //        if (this.IsInlineMatch)
    //        {
    //            return this.sourceKey.IsMatch(stringCmd);
    //        }
    //        return this.sourceValue.IsMatch(stringCmd);
    //    }

    //    //states invoke this from the worker thread when a known key/value pair is seen this update the Player/World objects
    //    internal void UpdateValue(WorkerState_InGame workerState, string stringCmd)
    //    {
    //        Match m;
    //        if (this.IsInlineMatch)
    //        {
    //            m = this.sourceKey.Match(stringCmd);
    //        }
    //        else
    //        {
    //            m = this.sourceValue.Match(stringCmd);
    //        }

    //        if (!m.Success)
    //        {
    //            return;
    //        }

    //        workerState.Engine.GameDataChange(this.TargetProperty, m.Groups);
    //    }
    //}
}