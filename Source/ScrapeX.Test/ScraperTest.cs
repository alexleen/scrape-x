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
    public class ScraperTest
    {
        private const string BaseUrl = "baseUrl";
        private IScraper mSut;

        [SetUp]
        public void SetUp()
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HTML\cl-apa-listing.html"));
            XPathNavigator navigator = htmlDoc.CreateNavigator();

            INavigatorFactory navigatorFactory = Substitute.For<INavigatorFactory>();
            navigatorFactory.Create(BaseUrl, Arg.Any<HttpClient>(), Arg.Any<HtmlWeb>()).Returns(navigator);

            mSut = new Scraper(BaseUrl, navigatorFactory);
        }

        [Test]
        public void UseHttpClient_ShouldThrow_WhenNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.UseHttpClient(null));
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
        public void Go_ShouldThrow_WhenCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.Go(null));
        }

        [Test]
        public void Go_ShouldThrow_WhenTargetPageXPathsAreNull()
        {
            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IScraper.SetTargetPageXPaths)}.");
        }

        [Test]
        public void Go_ShouldScrapeTarget()
        {
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            string receivedLink = null;
            IDictionary<string, string> values = null;
            mSut.Go((link, dict) =>
            {
                receivedLink = link;
                values = dict;
            });

            Assert.AreEqual(BaseUrl, receivedLink);
            Assert.AreEqual(1, values.Count);
            Assert.AreEqual("3BR", values["br"]);
        }
    }
}
