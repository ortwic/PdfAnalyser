using System.Collections.Generic;

namespace OCSoft.PdfAnalyser.Service.Interfaces
{
    public interface IConverter<TResult, TIn>
    {
        IEnumerable<TResult> Parse(TIn data);
    }
}