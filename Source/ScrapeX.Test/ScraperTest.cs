// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml.XPath;
using HtmlAgilityPack;
using NSubstitute;
using NUnit.Framework;
using ScrapeX.Interfaces;

namespace ScrapeX.Test
{
    [TestFixture]
    public class ScraperTest
    {
        private const string BaseUrl = "baseUrl";
        private XPathNavigator mNavigator;
        private INavigatorFactory mNavigatorFactory;
        private IScraper mSut;

        [SetUp]
        public void SetUp()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HTML\cl-apa-listing.html"));
            mNavigator = htmlDoc.CreateNavigator();

            mNavigatorFactory = Substitute.For<INavigatorFactory>();
            mNavigatorFactory.Create(BaseUrl, Arg.Any<HttpClient>(), Arg.Any<HtmlWeb>()).Returns(mNavigator);

            mSut = new Scraper(BaseUrl, mNavigatorFactory);
        }

        [Test]
        public void UseHttpClient_ShouldThrow_WhenNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.UseHttpClient(null));
        }

        [Test]
        public void UseHttpClient_ShouldUseHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            mSut.UseHttpClient(httpClient);

            //Set this up so it only responds to a call with this HttpClient
            mNavigatorFactory.Create(BaseUrl, httpClient, Arg.Any<HtmlWeb>()).Returns(mNavigator);

            Go_ShouldScrapeTarget();
        }

        [Test]
        public void SetTargetPageXPaths_ShouldThrow_WhenXPathsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.SetTargetPageXPaths(null));
        }

        [Test]
        public void SetTargetPageXPaths_ShouldThrow_WhenXPathsIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => mSut.SetTargetPageXPaths(new Dictionary<string, string>()));
        }

        [Test]
        public void Go_Individual_ShouldThrow_WhenCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.Go((Action<string, IDictionary<string, string>>)null));
        }

        [Test]
        public void Go_Tables_ShouldThrow_WhenCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.Go((Action<string, IDictionary<string, IEnumerable<IEnumerable<string>>>>)null));
        }

        [Test]
        public void Go_Both_ShouldThrow_WhenCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.Go((Action<string, IDictionary<string, string>, IDictionary<string, IEnumerable<IEnumerable<string>>>>)null));
        }

        [Test]
        public void Go_ShouldThrow_WhenTargetPageXPathsAreNull()
        {
            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict, tables) => { })).AndHasMessage($"Must first call either {nameof(IScraper.SetTargetPageXPaths)} or {nameof(IScraper.SetTableXPaths)}.");
        }

        [Test]
        public void Go_ShouldScrapeTarget()
        {
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            string receivedLink = null;
            IDictionary<string, string> values = null;
            mSut.Go((link, dict, tables) =>
                {
                    receivedLink = link;
                    values = dict;
                });

            Assert.AreEqual(BaseUrl, receivedLink);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("3BR", values["br"]);
        }

        [Test]
        public void Go_ShouldScrapeTarget_WhenTargetXPathReturnsNull()
        {
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]/li" } });

            string receivedLink = null;
            IDictionary<string, string> values = null;
            mSut.Go((link, dict, tables) =>
                {
                    receivedLink = link;
                    values = dict;
                });

            Assert.AreEqual(BaseUrl, receivedLink);
            Assert.AreEqual(1, values.Count);
            Assert.IsNull(values["br"]);
        }
    }
}
