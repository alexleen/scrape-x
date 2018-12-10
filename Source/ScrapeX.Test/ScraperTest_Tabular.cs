using HtmlAgilityPack;
using NSubstitute;
using NUnit.Framework;
using ScrapeX.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml.XPath;

namespace ScrapeX.Test
{
    [TestFixture]
    internal class ScraperTest_Tabular
    {
        private const string BaseUrl = "baseUrl";
        private XPathNavigator mNavigator;
        private INavigatorFactory mNavigatorFactory;
        private IScraper mSut;

        [SetUp]
        public void SetUp()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HTML\wiki-apple-cup.html"));
            mNavigator = htmlDoc.CreateNavigator();

            mNavigatorFactory = Substitute.For<INavigatorFactory>();
            mNavigatorFactory.Create(BaseUrl, Arg.Any<HttpClient>(), Arg.Any<HtmlWeb>()).Returns(mNavigator);

            mSut = new Scraper(BaseUrl, mNavigatorFactory);
        }

        [Test]
        public void SetTableXPaths_ShouldThrow_WhenXPathsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.SetTableXPaths(null));
        }

        [Test]
        public void SetTableXPaths_ShouldThrow_WhenXPathsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => mSut.SetTableXPaths(new Dictionary<string, IEnumerable<string>>()));
        }

        [Test]
        public void Go_ShouldScrapeSingleTable()
        {
            const string tableKey = "WSU Coaching";
            mSut.SetTableXPaths(new Dictionary<string, IEnumerable<string>>
            {
                {tableKey, new []
                    {
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[1]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[2]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[3]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[4]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[5]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[6]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[7]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[8]",
                    }
                }
            });

            string resultLink = null;
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = null;
            mSut.Go((link, dict, tables) =>
            {
                resultLink = link;
                result = tables;
            });

            Assert.AreEqual(BaseUrl, resultLink);
            AssertSingleTable(tableKey, result);
        }

        [Test]
        public void Go_ShouldScrapeSplitTable()
        {
            const string tableKey = "Game results";
            mSut.SetTableXPaths(new Dictionary<string, IEnumerable<string>>
            {
                {tableKey, new []
                    {
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[1]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[2]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[3]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[4]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[5]"
                    }
                }
            });

            string resultLink = null;
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = null;
            mSut.Go((link, dict, tables) =>
            {
                resultLink = link;
                result = tables;
            });

            Assert.AreEqual(BaseUrl, resultLink);
            AssertSplitTable(tableKey, result);
        }

        [Test]
        public void Go_ShouldScrapeSplitAndSingleTables()
        {
            const string tableKey1 = "Game results";
            const string tableKey2 = "WSU Coaching";
            mSut.SetTableXPaths(new Dictionary<string, IEnumerable<string>>
            {
                {tableKey1, new []
                    {
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[1]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[2]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[3]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[4]",
                        "//*[@id=\"mw-content-text\"]/div/table[3]/tbody/tr/td/table/tbody/tr/td[5]"
                    }
                },
                {tableKey2, new []
                    {
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[1]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[2]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[3]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[4]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[5]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[6]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[7]",
                        "//*[@id=\"mw-content-text\"]/div/table[5]/tbody/tr/td[8]",
                    }
                }
            });

            string resultLink = null;
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = null;
            mSut.Go((link, dict, tables) =>
            {
                resultLink = link;
                result = tables;
            });

            Assert.AreEqual(BaseUrl, resultLink);

            AssertSplitTable(tableKey1, result);

            AssertSingleTable(tableKey2, result);
        }

        private static void AssertSplitTable(string tableKey, IDictionary<string, IEnumerable<IEnumerable<string>>> result)
        {
            Assert.AreEqual(5, result[tableKey].Count());
            Assert.AreEqual(112, result[tableKey].ElementAt(0).Count());
            Assert.AreEqual(111, result[tableKey].ElementAt(1).Count());
            Assert.AreEqual(111, result[tableKey].ElementAt(2).Count());
            Assert.AreEqual(111, result[tableKey].ElementAt(3).Count());
            Assert.AreEqual(111, result[tableKey].ElementAt(4).Count());
        }

        private static void AssertSingleTable(string tableKey, IDictionary<string, IEnumerable<IEnumerable<string>>> result)
        {
            Assert.AreEqual(8, result[tableKey].Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(0).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(1).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(2).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(3).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(4).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(5).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(6).Count());
            Assert.AreEqual(14, result[tableKey].ElementAt(7).Count());
        }
    }
}
