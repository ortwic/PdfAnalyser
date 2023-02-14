using OCSoft.PdfAnalyser.Service.Interfaces;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OCSoft.PdfAnalyser.Service
{
    public class PdfSharpExtractor : ITextReader
    { 
        public IEnumerable<string> GetTextContent(string filename)
        {
            using (var doc = PdfReader.Open(filename, PdfDocumentOpenMode.Import))
            {
                foreach (PdfPage page in doc.Pages)
                {
                    yield return string.Join(' ', page.ExtractText());
                }

                //LoopPages(filename, doc);
            }
        }
        
        /* LoopPages
        
        private static void LoopPages(string filename, PdfDocument doc)
        {
            for (int i = 0; i < doc.PageCount; i++)
            {
                // Get page from 1st document
                PdfPage page = doc.PageCount > i ? doc.Pages[i] : new PdfPage();
                var content = page.Select(e => e.Value);

                // Write document file name and page number on each page
                WriteFileNameAndPageNumber(filename, i, page);
            }
        }

        private static void WriteFileNameAndPageNumber(string filename, int idx, PdfPage page1)
        {
            XFont font = new XFont("Verdana", 10, XFontStyle.Bold);
            XStringFormat format = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Far
            };

            var gfx = XGraphics.FromPdfPage(page1);
            var box = page1.MediaBox.ToXRect();
            box.Inflate(0, -10);
            gfx.DrawString(String.Format("{0} • {1}", filename, idx + 1), font, XBrushes.Red, box, format);
        }

        */
    }

    public static class PdfSharpExtensions
    {
        public static IEnumerable<string> ExtractText(this PdfPage page)
        {
            var content = ContentReader.ReadContent(page);
            var text = content.ExtractText();
            return text;
        }

        public static IEnumerable<string> ExtractText(this CObject cObject)
        {
            if (cObject is COperator)
            {
                var cOperator = cObject as COperator;
                if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
                    cOperator.OpCode.Name == OpCodeName.TJ.ToString())
                {
                    foreach (var cOperand in cOperator.Operands)
                        foreach (var txt in ExtractText(cOperand))
                            yield return txt;
                }
            }
            else if (cObject is CSequence)
            {
                var cSequence = cObject as CSequence;
                foreach (var element in cSequence)
                    foreach (var txt in ExtractText(element))
                        yield return txt;
            }
            else if (cObject is CString)
            {
                var cString = cObject as CString;
                yield return cString.Value;
            }
        }
    }
}
