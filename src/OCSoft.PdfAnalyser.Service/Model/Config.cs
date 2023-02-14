using System;
using System.Collections.Generic;

namespace OCSoft.PdfAnalyser.Service.Model
{
    [Serializable]
    public class Config
    {
        public string Name { get; set; }

        public string PdfFiles { get; set; }

        public string[] Export { get; set; }

        public string[] Strategy { get; set; }

        public Dictionary<string, string> SearchExpr { get; set; }
        
        public string[] ContentKeys { get; set; }

        public Dictionary<string, string[]> Categories { get; set; }

        public Dictionary<string, string> Keywords { get; set; }
    }
}
