using OCSoft.Common;
using OCSoft.PdfAnalyser.Service.Interfaces;
using OCSoft.PdfAnalyser.Service.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("OCSoft.PdfAnalyser.Test")]

namespace OCSoft.PdfAnalyser.Service
{
    public class TextToDataConverter : IConverter<Entry, string>
    {
        public static string Placeholder = Guid.NewGuid().ToString();

        private string _text;

        public Config Config { get; }

        public CultureInfo Culture { get; } = CultureInfo.InvariantCulture;

        public TextToDataConverter(Config config)
        {
            Config = config;
            if (!string.IsNullOrEmpty(config.Culture) 
                && Regex.IsMatch(config.Culture, "^[a-z]{2}"))
            {
                Culture = new CultureInfo(config.Culture);
            }
        }

        public IEnumerable<Entry> Parse(string data)
        {
            _text = data;
            var result = Config.SearchExpr
                .SelectMany(kv => ParseOne(_text, kv.Value))
                .Where(e => e.Total != 0 || e.Value != 0)
                .ToArray();
            _text = null;

            return result;
        }

        private IEnumerable<Entry> ParseOne(string text, string pattern)
        {
            return new Regex(pattern, RegexOptions.Singleline)
                .Matches(text)
                .OfType<Match>()
                .Select(ToEntry);
        }

        private Entry ToEntry(Match match)
        {
            var entry = new Entry
            {
                Account = Config.Name,
                Booking = FormatDateString(match.Groups["booking"].Value, "yyyy-MM-dd"),
                Date = FormatDateString(match.Groups["date"].Value, "yyyy-MM-dd"),
                Total = FormatCurrency(match.Groups["total"].Value),
                Value = FormatCurrency(match.Groups["value"].Value),
                Raw = match.Groups["content"].Value
            };

            _text = _text.Replace(match.Value, "");
            return entry;
        }

        private string FormatDateString(string date, string format)
        {
            if (!string.IsNullOrEmpty(date) &&
                DateTime.TryParse(date, Culture, DateTimeStyles.None, out DateTime result))
            {
                return result.ToString(format);
            }
            return string.Empty;
        }

        private double FormatCurrency(string value)
        {
            double.TryParse(value, NumberStyles.Currency, Culture, out double result);
            return result;
        }
    }
}
