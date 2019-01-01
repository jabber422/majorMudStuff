using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    internal static class MyRegex
    {
        internal static Dictionary<Type, List<RegexAndResponse>> Cache;
        internal static RegexAndResponse HpEquals;
        static MyRegex()
        {
            Cache = new Dictionary<Type, List<RegexAndResponse>>();
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

    internal static class AnsiColorRegexTable
    {
        internal static Dictionary<Type, List<AnsiColorRegex>> Cache;

        static AnsiColorRegexTable()
        {
            Cache = new Dictionary<Type, List<AnsiColorRegex>>();
            List<AnsiColorRegex> ansiColorRegexs = new List<AnsiColorRegex>();
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Name: $", @"(\w+ (\w+)?\s+)", typeof(GameProcessorState_Green), "Player.Stats.Name", "Stats.Start"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Lives\/CP:\s+$", @"^\s+(\d+)\/(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.LivesCP"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Race: $", @"^(\w+)\s*$", typeof(GameProcessorState_Green), "Player.Stats.Race"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Exp: $", @"^(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Exp"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Perception:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Perception"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Class:\s+$", @"^(\w+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Class"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Level:\s+$", @"^(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Level"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Stealth:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Stealth"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Hits: $", @"^\s+(\d+)\/(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Hits"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Armour Class: $", @"^ (\d+)\/(\d+) $", typeof(GameProcessorState_Green), "Player.Stats.ArmourClass"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Thievery: $", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Thievery"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Traps:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Traps"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^\s+Picklocks:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Picklocks"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Strength:\s+$", @"^\s+(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Strength"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Agility:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Agility"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Tracking:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.Tracking"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Intellect:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Intellect"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Health:\s+$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Health"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Martial Arts:+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.MartialArts"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Willpower:$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Willpower"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^Charm:\s+$", @"^\s(\d+)\s+$", typeof(GameProcessorState_Green), "Player.Stats.Charm"));
            ansiColorRegexs.Add(new AnsiColorRegex(@"^MagicRes:\s+$", @"^\s+(\d+)$", typeof(GameProcessorState_Green), "Player.Stats.MagicRes", "Stats.End"));
            

            ansiColorRegexs.Add(new AnsiColorRegex(@"^Obvious exits: (.*)$", "", typeof(GameProcessorState_Green), "Player.Room.ObviousExits"));
            Cache.Add(typeof(GameProcessorState_Green), ansiColorRegexs);


            List<AnsiColorRegex> cyan = new List<AnsiColorRegex>();
            cyan.Add(new AnsiColorRegex(@"^\[HP=$", @"^(\d+)$", typeof(GameProcessorState_Cyan), "Player.Stats.CurHits"));
            cyan.Add(new AnsiColorRegex(@"^\]:", @"", typeof(GameProcessorState_Cyan), "Ignored.PlayerHealthCloseTag"));

            cyan.Add(new AnsiColorRegex(@"^You notice (.*)", "", typeof(GameProcessorState_Cyan), "Player.Room.ObviousItems"));

            Cache.Add(typeof(GameProcessorState_Cyan), cyan);

            List<AnsiColorRegex> boldCyan = new List<AnsiColorRegex>();
            Cache.Add(typeof(GameProcessorState_BoldCyan), boldCyan);
        }
    }

    //0 32 0
    //represent a regex that is checked from the corresponding state that matches the bold, foreground, background
    internal class AnsiColorRegex
    {
        string keyPattern;
        string valuePattern;
        internal string KeyPattern { get { return keyPattern; } set { if (sourceKey == null) sourceKey = new Regex(value); keyPattern = value; } }
        internal string ValuePattern { get { return valuePattern; } set { if (sourceValue == null) sourceValue = new Regex(value); valuePattern = value; } }
        Regex sourceKey;
        Regex sourceValue;
        Type GameProcessorState_Type;
        string TargetProperty;

        public string Block;
        public bool IsInlineMatch = false;

        internal AnsiColorRegex(string srcKeyPattern, string srcValuePattern, Type t, string targetProperty, string block = "")
        {
            this.KeyPattern = srcKeyPattern;
            this.ValuePattern = srcValuePattern;

            if (this.ValuePattern == "")
            {
                this.IsInlineMatch = true;
            }


            this.GameProcessorState_Type = t;
            this.TargetProperty = targetProperty;
            this.Block = block;
        }

        internal bool IsKeyMatch(TermStringDataCmd stringCmd)
        {
            return this.sourceKey.IsMatch(stringCmd.GetValue());
        }

        internal bool IsValueMatch(TermStringDataCmd stringCmd)
        {
            if (this.IsInlineMatch)
            {
                return this.sourceKey.IsMatch(stringCmd.GetValue());
            }
            return this.sourceValue.IsMatch(stringCmd.GetValue());
        }

        //states invoke this from the worker thread when a known key/value pair is seen this update the Player/World objects
        internal void UpdateValue(WorkerState_InGame workerState, TermStringDataCmd stringCmd)
        {
            Match m = this.sourceValue.Match(stringCmd.GetValue());
            if (!m.Success)
            {
                return;
            }

            workerState.Engine.GameDataChange(this.TargetProperty, m.Groups);
        }
    }
}