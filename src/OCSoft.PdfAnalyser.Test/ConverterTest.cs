using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSoft.PdfAnalyser.Service;
using OCSoft.PdfAnalyser.Service.Model;
using System.Collections.Generic;
using System.Linq;

namespace OCSoft.PdfAnalyser.Test
{
    [TestClass]
    public class ConverterTest
    {
        private readonly Config _config = new Config
        {
            Culture = "de-DE",
            SearchExpr = new Dictionary<string, string>
            {
                { "0", "[*]{5}\\sf00\\sb4r\\s(?<date>\\d{2}\\.\\d{2}\\.\\d{2})\\s[*]{5}\\s+(?<total>-?[\\.\\d]+,\\d{2}).+?RANDOM5.+?[*]{3}" },
                { "1", "((?<booking>\\d{2}\\.\\d{2}\\.\\d{4})\\s(?<content>.+?)\\s+(?<date>\\d{2}\\.\\d{2}\\.\\d{4})\\s+(?<value>-?[\\.\\d]+,\\d{2}))" },
            },
            ContentKeys = new[] { "KEY\\d+", "\\wREF+", "ABWE+", "SVWZ+" },
            Categories = new Dictionary<string, string[]>
            {
                { "KFZ", new [] { "Auto", "Kfz.+", "ein-beitrag" } }
            },
            Keywords = new Dictionary<string, string>
            {
            }
        };

        [TestMethod]
        [DynamicData(nameof(SampleEntries))]
        public void ParsePatterns_Test(Entry expected, string text)
        {
            var converter = new TextToDataConverter(_config);

            var result = converter.Parse(text).SingleOrDefault();

            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Booking, result.Booking);
            Assert.AreEqual(expected.Date, result.Date);
            Assert.AreEqual(expected.Month, result.Month);
            Assert.AreEqual(expected.Total, result.Total);
            Assert.AreEqual(expected.Value, result.Value);
        }

        private static IEnumerable<object[]> SampleEntries 
        {
            get 
            {
                yield return new object[]
                {
                    new Entry
                    {
                        Booking = "",
                        Date = "2000-09-30",
                        Month = "Sep 2000",
                        Total = -123.45
                    },
                    "***** f00 b4r 30.09.00 *****  -123,45 550668 EREF+ K11078550668/000020/RANDOM5 *** "
                };
                yield return new object[] 
                {
                    new Entry
                    {
                        Booking = "2000-09-30",
                        Date = "2000-09-30",
                        Month = "Sep 2000",
                        Value = -123.45
                    },
                    "30.09.2000 \r\nher3 is S0M3 random date: 29.09.2000 plus another \r\n01.09.2000 +NUMS K11078550668 EREF+ K11078550668/000020/19ZVS06F 30.09.2000 \n-123,45"
                };
            }
        }

        [TestMethod]
        [DataRow("25.10.2019 Some random text SVWZ+ other stuff/or mouse - Foo Bar 10 / 2019 EREF+ 6929610439 - 000017 25.10.2019 3.269, 30")]
        public void ParseKeyValues_Test(string text)
        {
            var converter = new DataTagger(_config);

            var dict = converter.ParseKeyValues(text);

            CollectionAssert.AreEquivalent(new[] { "", "SVWZ+", "EREF+" }, dict.Keys);
        }

        [TestMethod]
        [DataRow("", "Ente")]
        [DataRow("KFZ", "Auto")]
        [DataRow("KFZ", "AUTO")]
        public void GetCategoryByKey_Test(string expected, string key)
        {
            var converter = new DataTagger(_config);

            var result = converter.GetCategory(new string[] { key }, string.Empty);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("", "")]
        [DataRow("", "No match, nada, nothing")]
        [DataRow("KFZ", "Mein Auto bla/0589")]
        [DataRow("KFZ", "Mein AUTO bla/0589")]
        [DataRow("KFZ", "Kfz-Meisterbetrieb")]
        [DataRow("KFZ", "der ein-beitrag im 033458")]
        public void GetCategoryBySubject_Test(string expected, string subject)
        {
            var converter = new DataTagger(_config);

            var result = converter.GetCategory(new string[0], subject);

            Assert.AreEqual(expected, result);
        }
    }
}
