using System.Text.RegularExpressions;

namespace MMudTerm_Protocols.Engine
{
    internal class RegexAndResponse
    {
        public Regex regex;
        public string Reponse;
        public bool IsMatched = false;

        public RegexAndResponse(string regexString, string response)
        {
            this.regex = new Regex(regexString);
            this.Reponse = response;
        }

        public bool IsMatch(TermStringDataCmd cmd)
        {
            return regex.Match(cmd.GetValue()).Success;
        }
    }

    internal class RegexAndResponseAndState : RegexAndResponse
    {
        GameProcessorState NextState;
        public bool isMatched = false;

        public RegexAndResponseAndState(string regexString, string response, GameProcessorState nextState) : base(regexString, response)
        {
            this.regex = new Regex(regexString);
            this.Reponse = response;
        }

        //public bool IsMatch(TermStringDataCmd cmd)
        //{
        //    return regex.Match(cmd.GetValue()).Success;
        //}
    }
}