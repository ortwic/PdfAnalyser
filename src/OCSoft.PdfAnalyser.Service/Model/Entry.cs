using OCSoft.Common.Extensions;
using System.Collections.Generic;

namespace OCSoft.PdfAnalyser.Service.Model
{
    public class Entry
    {
        public string Booking { get; set; }

        public string Date { get; set; }

        public double Total { get; set; }

        public double Value { get; set; }

        public string Typ { get; set; }

        public string Category { get; set; }

        public string Subject { get; set; }

        public string Usage { get; set; }

        public Dictionary<string, string> Contents { get; set; }

        public string Raw { get; set; }
    }
}
