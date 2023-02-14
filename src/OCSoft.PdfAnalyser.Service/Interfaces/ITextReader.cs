using System.Collections.Generic;

namespace OCSoft.PdfAnalyser.Service.Interfaces
{
    public interface ITextReader
    {
        IEnumerable<string> GetTextContent(string filename);
    }
}