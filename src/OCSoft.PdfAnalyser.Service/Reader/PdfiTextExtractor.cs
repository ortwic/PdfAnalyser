using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using OCSoft.PdfAnalyser.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace OCSoft.PdfAnalyser.Service
{
    public abstract class PdfiTextExtractor : ITextReader
    {
        public class CustomContentOperator : IContentOperator
        {
            public void Invoke(PdfCanvasProcessor processor, PdfLiteral @operator, IList<PdfObject> operands)
            {

            }
        }

        #region Fields

        readonly ITextExtractionStrategy _strategy;
        readonly Dictionary<string, IContentOperator> _operators = new Dictionary<string, IContentOperator>()
        {
            { nameof(CustomContentOperator), new CustomContentOperator() }
        };

        #endregion

        protected PdfiTextExtractor(ITextExtractionStrategy strategy)
        {
            _strategy = strategy;
        }
        
        public IEnumerable<string> GetTextContent(string filename)
        {
            using (var reader = new PdfReader(filename))
            {
                using (var doc = new PdfDocument(reader))
                {
                    return AsPdfPages(doc)
                        .Select(p => PdfTextExtractor.GetTextFromPage(p, _strategy, _operators))
                        .ToArray();
                }
            }
        }

        private static IEnumerable<PdfPage> AsPdfPages(PdfDocument doc)
        {
            for (int i = 1; i <= doc.GetNumberOfPages(); i++)
            {
                yield return doc.GetPage(i);
            }
        }
    }
}
