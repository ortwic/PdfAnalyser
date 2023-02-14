using OCSoft.Common;
using OCSoft.PdfAnalyser.Service.Model;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("OCSoft.PdfAnalyser.Test")]

namespace OCSoft.PdfAnalyser.Service
{
    public class DataTagger //: IConverter<Entry, Entry>
    {
        public const string SUBJECT_KEY = "";

        private readonly string _usageKey;

        public Config Config { get; }

        public DataTagger(Config config)
        {
            Config = config;
            _usageKey = config.ContentKeys.FirstOrDefault();
        }

        public Entry Tag(Entry data)
        {
            var contents = ParseKeyValues(data.Raw);
            data.Subject = contents[SUBJECT_KEY];
            data.Usage = contents.GetValueOrDefault(_usageKey, "");
            data.Contents = contents;
            data.Raw = RemoveLineBreaks(data.Raw);

            data.Category = GetCategory(contents.Keys, $"{data.Subject} {data.Usage}");
            data.Typ = GetEntryType(data);

            contents.Remove(SUBJECT_KEY);
            if (contents.ContainsKey(_usageKey))
            {
                contents.Remove(_usageKey);
            }

            return data;
        }

        internal Dictionary<string, string> ParseKeyValues(string text)
        {
            string separators = string.Join('|', Config.ContentKeys).Replace("+", "\\+");
            var match = new Regex($"({separators})", RegexOptions.Singleline).Split(text);
            var dict = new Dictionary<string, string>(ToKeyValues(match));
            return dict;
        }

        private static IEnumerable<KeyValuePair<string, string>> ToKeyValues(string[] match)
        {
            for (int i = 0; i < match.Length; ++i)
            {
                string key = i == 0 ? SUBJECT_KEY : match[i++].Trim();
                if (i == match.Length)
                {
                    EventLogger.Warning($"No value for '{key}'!");
                    yield break;
                }
                string value = RemoveLineBreaks(match[i]);
                yield return new KeyValuePair<string, string>(key, value);
            }
        }

        internal string GetCategory(IEnumerable<string> keys, string subject)
        {
            var upperKeys = keys.Select(k => k.ToUpper()).ToArray();
            foreach (var cat in Config.Categories)
            {
                if (cat.Value.Any(p => upperKeys.Contains(p.ToUpper())
                || Regex.IsMatch(subject, p, RegexOptions.IgnoreCase)))
                {
                    return cat.Key;
                }
            }

            return string.Empty;
        }

        private string GetEntryType(Entry entry)
        {
            var type = Config.Keywords.Where(kv => Regex.IsMatch(entry.Subject, kv.Value))
                .Select(kv => (KeyValuePair<string, string>?)kv).FirstOrDefault();

            if (type.HasValue)
            {
                entry.Subject = Regex.Replace(entry.Subject, type.Value.Value, "");
                return type.Value.Key;
            }

            return string.Empty;
        }

        private static string RemoveLineBreaks(string text)
        {
            return Regex.Replace(text, "\\s+", " ");
        }
    }
}
