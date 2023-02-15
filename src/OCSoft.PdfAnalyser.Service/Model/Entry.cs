using OCSoft.Common.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace OCSoft.PdfAnalyser.Service.Model
{
    [DebuggerDisplay("Bk={Booking};Dt={Date};Tot={Total};Val={Value};Sbj={Subject}")]
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
