using MMudTerm_Protocols.AnsiProtocolCmds;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    internal abstract class ColorKeyColorValue
    {
        //this is a repeating pattern
        //(cyan)[HP=(bright cyan)1234
        //(cyan)[MP=(bright cyan)1234
        //(green)Name:(white)Bob
        internal abstract ColorKeyColorValue DoWork(Engine eng, TermCmd cmd);
    }

    internal class ColorKeyColorValue_StatsKeyColor : ColorKeyColorValue
    {
        int Attribute = 0;
        int Foreground = 32;

        internal override ColorKeyColorValue DoWork(Engine eng, TermCmd cmd)
        {
            if (cmd is AnsiGraphicsCmd)
            {
                AnsiGraphicsCmd graphicsCmd = (cmd as AnsiGraphicsCmd);
                if(graphicsCmd.vals[0] == Attribute && graphicsCmd.vals[1] == Foreground)
                {
                    return new ColorKeyColorValue_StatsKey();
                }
            }
            return this;
        }
    }

    internal class ColorKeyColorValue_StatsKey : ColorKeyColorValue
    {
        int Attribute = 0;
        int Foreground = 32;

        internal override ColorKeyColorValue DoWork(Engine eng, TermCmd cmd)
        {
            if (cmd is TermStringDataCmd)
            {
                TermStringDataCmd stringCmd = (cmd as TermStringDataCmd);
                Match m = Regex.Match(stringCmd.GetValue(), @"^(.*):");
                if (m.Success)
                {
                    string key = m.Groups[1].Value;
                    ColorKeyColorValue nextState = new ColorKeyColorValue_StatsValueColor(key);
                }
                
            }
            return this;
        }
    }

    internal class ColorKeyColorValue_StatsValueColor : ColorKeyColorValue
    {
        int Attribute = 0;
        int Foreground = 37;
        private string key;

        public ColorKeyColorValue_StatsValueColor(string key)
        {
            this.key = key;
        }

        internal override ColorKeyColorValue DoWork(Engine eng, TermCmd cmd)
        {
            if (cmd is TermStringDataCmd)
            {
                TermStringDataCmd stringCmd = (cmd as TermStringDataCmd);
                Match m = Regex.Match(stringCmd.GetValue(), @"^(.*):");
                if (m.Success)
                {
                    string key = m.Groups[1].Value;
                    ColorKeyColorValue nextState = new ColorKeyColorValue_StatsValueColor(key);
                }

            }
            return this;
        }
    }
}