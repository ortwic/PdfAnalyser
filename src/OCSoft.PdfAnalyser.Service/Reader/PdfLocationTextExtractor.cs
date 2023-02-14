using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace OCSoft.PdfAnalyser.Service
{
    public class PdfLocationTextExtractor : PdfiTextExtractor
    {
        //public PdfLocationTextExtractor(string regex)
        //    : base(CreateStrategy(regex))
        //{
        //}

        //private static LocationTextExtractionStrategy CreateStrategy(string regex)
        //{
        //    var s = new RegexBasedLocationExtractionStrategy(regex);
        //    return new LocationTextExtractionStrategy();
        //}

        public PdfLocationTextExtractor()
            : base(new LocationTextExtractionStrategy())
        {
        }
    }
}
