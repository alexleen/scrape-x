using HtmlAgilityPack;
using NSubstitute;
using NUnit.Framework;
using ScrapeX.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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
        public void Go_ShouldScrapeSingleTable()
        {
            Assert.Ignore();
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

            string resultLink;
            IDictionary<string, IList<IList<string>>> result = null;
            mSut.GoTables((link, dict) =>
            {
                resultLink = link;
                result = dict;
            });

            Assert.AreEqual(5, result[tableKey].Count);
            Assert.AreEqual(112, result[tableKey][0].Count);
            Assert.AreEqual(111, result[tableKey][1].Count);
            Assert.AreEqual(111, result[tableKey][2].Count);
            Assert.AreEqual(111, result[tableKey][3].Count);
            Assert.AreEqual(111, result[tableKey][4].Count);
        }
    }
}
