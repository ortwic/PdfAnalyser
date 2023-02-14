using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace OCSoft.PdfAnalyser.Service
{
    public class PdfSimpleTextExtractor : PdfiTextExtractor
    {
        public PdfSimpleTextExtractor()
            : base(new SimpleTextExtractionStrategy())
        {
        }
    }
}
