using BitMiracle.Docotic.Pdf;
using OCSoft.PdfAnalyser.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCSoft.PdfAnalyser.Service
{
    public class PdfBitMiracleExtractor : ITextReader
    {
        public IEnumerable<string> GetTextContent(string filename)
        {
            using (PdfDocument pdf = new PdfDocument())
            {
                pdf.Open(filename);

                PdfCanvas canvas = pdf.Pages[0].Canvas;
                //canvas.Font = pdf.AddFont(PdfBuiltInFont.Helvetica);
                //canvas.FontSize = 20;
                //canvas.DrawString(10, 80, "This text added by Docotic.Pdf Sample Browser");

            }

            throw new NotImplementedException();
        }
    }
}
