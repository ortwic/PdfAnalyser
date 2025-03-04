using OCSoft.Common;
using OCSoft.PdfAnalyser.Service;
using System;
using System.Linq;

namespace OCSoft.PdfAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLogger.Instance.OnDebug += (s, v) => Console.Write(v);

            if (args.Any())
            {
                var batch = new Batch(args[0]);
                batch.Run();
            }
            else
            {
                Console.WriteLine("No config file specified.");
            }

            Console.WriteLine("Press enter to quit.");
            Console.Read();
        }
    }
}
