using MMudObjects;
using MMudTerm.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace MMudTerm.Session
{
    internal static class MessagesCache
    {
        static List<MessageResponse> messages = null;
        static public bool Loaded = false;

        public static List<MessageResponse> Messages { get { if (messages != null) { return messages; } else { Load();return messages; }; } }

        internal static void Load()
        {
            Loaded = false;
            messages = new List<MessageResponse>();
            var d = Directory.GetCurrentDirectory();
            var p = Path.Combine(d, "res", "Messages.md");
            using (StreamReader sr = new StreamReader(p))
            {
                while (!sr.EndOfStream)
                {
                    var one = sr.ReadLine();
                    var two = sr.ReadLine();
                    var three = sr.ReadLine();
                    MessageResponse mr = new MessageResponse(one);
                    mr.Message = two;
                    mr.EndsWith = three;
                    messages.Add(mr);
                }
            }

            Loaded = true;
        }

        static public Dictionary<string, List<(EventType, Object)>> RegexListwMacros()
        {
            Dictionary<string, List<(EventType, Object)>> message_patterns = new Dictionary<string, List<(EventType, Object)>>();
            foreach (var message in Messages)
            {
                if (message.EndsWith.StartsWith("You slow"))
                {

                }
                if (message.EndsWith == null || message.EndsWith == string.Empty)
                {
                    //only the message string exits, these seems like messages or damage spells
                    if (message_patterns.ContainsKey(message.Message))
                    {
                        message_patterns[message.Message].Add((EventType.MessageResponse, message));
                    }
                    else
                    {
                        message_patterns.Add(message.Message,
                            new List<(EventType, Object)> { (EventType.MessageResponse, message) });
                    }
                }
                else
                {
                    if (message_patterns.ContainsKey(message.Message))
                    {
                        message_patterns[message.Message].Add((EventType.MessageResponseBuffStart, message));
                    }
                    else
                    {
                        message_patterns.Add(message.Message,
                            new List<(EventType, Object)> { (EventType.MessageResponseBuffStart, message) });
                    }
                    //both message and endswith exist, these all seems to be buff/debuff messages
                    if (message_patterns.ContainsKey(message.EndsWith))
                    {
                        message_patterns[message.EndsWith].Add((EventType.MessageResponseBuffEnd, message));
                    }
                    else
                    {
                        message_patterns.Add(message.EndsWith,
                            new List<(EventType, Object)> { (EventType.MessageResponseBuffEnd, message) });
                    }
                }
            }
            return message_patterns;
        }
    }
}