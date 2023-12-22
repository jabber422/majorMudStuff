using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MMudObjects
{
    //access to global title -> lvl range
    public static class ClassTitles
    {
        static Dictionary<string, Dictionary<string, string>> _dict;
        static Dictionary<string, Dictionary<Regex, string>> _optimized_dict;

        static ClassTitles()
        {
            using (FileStream fs = new FileStream("Titles.json", FileMode.Open))
            {
                using (TextReader rdr = new StreamReader(fs))
                {
                    _dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(rdr.ReadToEnd());
                }
            }

            _optimized_dict = new Dictionary<string, Dictionary<Regex, string>>();
            foreach (string className in _dict.Keys)
            {
                foreach (string title in _dict[className].Keys)
                {
                    Regex r = new Regex(title);
                    if (_optimized_dict.ContainsKey(className))
                    {
                        _optimized_dict[className].Add(r, _dict[className][title]);
                    }
                    else
                    {
                        Dictionary<Regex, string> dict = new Dictionary<Regex, string>();
                        dict.Add(r, _dict[className][title]);
                        _optimized_dict.Add(className, dict);
                    }
                }
            }
        }

        public static string GetLevelRangeByTitle(string title)
        {
            foreach(Dictionary<Regex, string> dict in _optimized_dict.Values)
            {
                foreach(Regex r in dict.Keys)
                {
                    if (r.IsMatch(title))
                    {
                        return dict[r];
                    }
                }
            }
            return "";
        }

        public static string GetLevelRangeByClassAndTitle(PlayableClass cls, string title)
        {
            if (cls == null) return GetLevelRangeByTitle(title);
            if (_optimized_dict.ContainsKey(cls.Name))
            {
                foreach (Regex r in _optimized_dict[cls.Name].Keys)
                {
                    if (r.IsMatch(title))
                    {
                        return _optimized_dict[cls.Name][r];
                    }
                }
            }
            return "";
        }
    }
}
