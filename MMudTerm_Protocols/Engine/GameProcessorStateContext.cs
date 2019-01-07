using MMudObjects;

namespace MMudTerm_Protocols.Engine
{
    internal class GameProcessorStateContext
    {
        internal int Attribute { get; set; }
        internal int Foreground { get; set; }
        internal int Background { get; set; }

        //internal AnsiColorRegex LastRegex { get; set; }
        internal MatchAndCapture MatchAndCapture { get; set; }
    }
}