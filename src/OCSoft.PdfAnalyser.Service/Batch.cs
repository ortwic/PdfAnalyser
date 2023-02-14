using OCSoft.Common;
using OCSoft.Common.Extensions;
using OCSoft.PdfAnalyser.Service.Interfaces;
using OCSoft.PdfAnalyser.Service.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OCSoft.PdfAnalyser.Service
{
    public class Batch
    {
        private readonly FileInfo _configfile;
        private readonly Lazy<Config> _lazyConfig;

        public string Workspace => _configfile.DirectoryName;

        private string _name;

        public string Name => _name ?? (_name = GetName());

        private string GetName()
        {
            string name = _lazyConfig.Value.Name;
            return !string.IsNullOrEmpty(name) ? name : _configfile.Name.Replace(_configfile.Extension, "");
        }

        public Config Config => _lazyConfig.Value;

        public TextToDataConverter Converter => new TextToDataConverter(_lazyConfig.Value);

        public DataTagger Tagger => new DataTagger(_lazyConfig.Value);

        public Batch(string configfile)
        {
            _configfile = new FileInfo(configfile);
            _lazyConfig = new Lazy<Config>(() => ReadConfig(configfile));
        }

        private static Config ReadConfig(string configfile)
        {
            try
            {
                string text = File.ReadAllText(configfile);
                return new Deserializer().Deserialize<Config>(text);
            }
            catch (Exception ex)
            {
                EventLogger.Error($"Error reading config!\n {ex}");
                return null;
            }
        }

        public void Run()
        {
            if (_lazyConfig.Value == null)
            {
                return;
            }

            EventLogger.Debug($"Find pdf files in {Workspace}.");
            string[] files = GetPdfFilesRecursivly(Workspace);

            foreach (string strategy in _lazyConfig.Value.Strategy)
            {
                EventLogger.Trace($"Reading pdf files with {strategy} strategy.");
                switch (strategy)
                {
                    case "sharp":
                        ProcessFiles<PdfSharpExtractor>(files);
                        break;

                    case "location":
                        ProcessFiles<PdfLocationTextExtractor>(files);
                        break;

                    case "simple":
                    default:
                        ProcessFiles<PdfSimpleTextExtractor>(files);
                        break;
                }
                EventLogger.Instance.Watch.Restart();
            }
        }

        private string[] GetPdfFilesRecursivly(string dir)
        {
            return Directory.GetDirectories(dir)
                .SelectMany(GetPdfFilesRecursivly)
                .Union(Directory.GetFiles(dir, $"{_lazyConfig.Value.PdfFiles}.pdf"))
                .ToArray();
        }

        private void ProcessFiles<TReader>(string[] files) where TReader : ITextReader, new()
        {
            try
            {
                string[] text = files.AsParallel()
                    .SelectMany(f => new TReader().GetTextContent(f))
                    .ToArray();

                if (_lazyConfig.Value.Export.Contains("txt"))
                {
                    Task.Run(() => ExportToFile(text, "txt"));
                }

                EventLogger.Trace($"Parsing text contents...");
                if (_lazyConfig.Value.Export.Contains("csv"))
                {
                    var entries = text.SelectMany(t => Converter.Parse(t)).OrderBy(e => e.Date).ToArray();
                    EventLogger.Trace($"Parsed {entries.Length} items.");

                    Parallel.ForEach(entries, e => Tagger.Tag(e));
                    EventLogger.Trace($"Tagged {entries.Length} items.");

                    var data = entries.Select(e => e.PropertiesToString(PropertyFormat.ListValue));
                    var csv = new string[] { new Entry().PropertiesToString(PropertyFormat.ListColumn) }.Union(data).ToArray();
                    ExportToFile(csv, "csv");
                }
            }
            catch (Exception ex)
            {
                EventLogger.Error($"{typeof(TReader).Name} failed: {ex.Message}");
            }
        }

        private void ExportToFile(IEnumerable<string> data, string ext)
        {
            if (data.Any())
            {
                string path = Path.Combine(Workspace, $"{Name}.{ext}");
                File.WriteAllText(path, string.Join(Environment.NewLine, data), Encoding.UTF8);

                EventLogger.Trace($"exported to {path}");
            }
        }
    }
}
