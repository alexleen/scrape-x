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
    public class PaginatingScraperTest
    {
        private const string BaseUrl = "url";
        private const string ResultsStartPage = "/search/apa";
        private IPaginatingScraper mSut;

        [SetUp]
        public void SetUp()
        {
            HtmlDocument targetHtmlDoc = new HtmlDocument();
            targetHtmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HTML\cl-apa-listing.html"));
            XPathNavigator targetNav = targetHtmlDoc.CreateNavigator();

            HtmlDocument searchHtmlDoc = new HtmlDocument();
            searchHtmlDoc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"HTML\cl-apa-search-page.html"));
            XPathNavigator searchNav = searchHtmlDoc.CreateNavigator();

            INavigatorFactory navigatorFactory = Substitute.For<INavigatorFactory>();
            navigatorFactory.Create(BaseUrl + ResultsStartPage, Arg.Any<HttpClient>(), Arg.Any<HtmlWeb>()).Returns(searchNav);
            //For all other urls, return target page
            navigatorFactory.Create(Arg.Is<string>(url => url != BaseUrl + ResultsStartPage), Arg.Any<HttpClient>(), Arg.Any<HtmlWeb>()).Returns(targetNav);

            mSut = new PaginatingScraper(BaseUrl, navigatorFactory);
        }

        /// <summary>
        /// Strings that will fail the string.IsNullOrWhiteSpace(...) check.
        /// </summary>
        private static readonly IEnumerable<TestCaseData> sInvalidStrings = new[] { new TestCaseData(null), new TestCaseData(string.Empty), new TestCaseData(" ") };

        /// <summary>
        /// Strings that will fail XPathExpression.Compile(...)
        /// </summary>
        private static readonly IEnumerable<TestCaseData> sInvalidXPaths = new[] { new TestCaseData("-") };

        [TestCaseSource(nameof(sInvalidStrings))]
        public void Ctor_ShouldThrow_WhenInvalidBaseUrl(string invalidUrl)
        {
            Assert.DoesNotThrow(() => new PaginatingScraper(invalidUrl, null));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetResultsStartPage_ShouldThrow_WhenInvalidUrl(string invalidUrl)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetResultsStartPage(invalidUrl));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetNextLink_ShouldThrow_WhenInvalidString(string invalidXPath)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetNextLinkXPath(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidXPaths))]
        public void SetNextLink_ShouldThrow_WhenInvalidXPath(string invalidXPath)
        {
            Assert.Throws<XPathException>(() => mSut.SetNextLinkXPath(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetIndividualResultNodeXPath_ShouldThrow_WhenInvalidString(string invalidXPath)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetIndividualResultNodeXPath(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidXPaths))]
        public void SetIndividualResultNodeXPath_ShouldThrow_WhenInvalidXPath(string invalidXPath)
        {
            Assert.Throws<XPathException>(() => mSut.SetIndividualResultNodeXPath(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetIndividualResultLinkXPath_ShouldThrow_WhenInvalidString(string invalidXPath)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetIndividualResultLinkXPath(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidXPaths))]
        public void SetIndividualResultLinkXPath_ShouldThrow_WhenInvalidXPath(string invalidXPath)
        {
            Assert.Throws<XPathException>(() => mSut.SetIndividualResultLinkXPath(invalidXPath));
        }

        [Test]
        public void SetResultVisitPredicate_ShouldThrow_WhenPredicateIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.SetResultVisitPredicate(null, "whatev"));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetResultVisitPredicate_ShouldThrow_WhenInvalidString(string invalidXPath)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetResultVisitPredicate(str => true, invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidXPaths))]
        public void SetResultVisitPredicate_ShouldThrow_WhenInvalidXPath(string invalidXPath)
        {
            Assert.Throws<XPathException>(() => mSut.SetResultVisitPredicate(str => true, invalidXPath));
        }

        [Test]
        public void Go_ShouldThrow_WhenCallbackIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.Go(null));
        }

        [Test]
        public void Go_ShouldThrow_WhenResultsStartPageUrlIsNull()
        {
            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IPaginatingScraper.SetResultsStartPage)}.");
        }

        [Test]
        public void Go_ShouldThrow_WhenIndividualNodeXPathIsNull()
        {
            mSut.SetResultsStartPage("/");

            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IPaginatingScraper.SetIndividualResultNodeXPath)}.");
        }

        [Test]
        public void Go_ShouldThrow_WhenIndividualLinkXPathIsNull()
        {
            mSut.SetResultsStartPage("/");
            mSut.SetIndividualResultNodeXPath("/");

            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IPaginatingScraper.SetIndividualResultLinkXPath)}.");
        }

        [Test]
        public void Go_ShouldThrow_WhenNextLinkXPathIsNull()
        {
            mSut.SetResultsStartPage("/");
            mSut.SetIndividualResultNodeXPath("/");
            mSut.SetIndividualResultLinkXPath("/");

            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IPaginatingScraper.SetNextLinkXPath)}.");
        }

        [Test]
        public void Go_ShouldThrow_WhenTargetPageXPathsAreNull()
        {
            mSut.SetResultsStartPage("/");
            mSut.SetIndividualResultNodeXPath("/");
            mSut.SetIndividualResultLinkXPath("/");
            mSut.SetNextLinkXPath("/");

            Assert.Throws<InvalidOperationException>(() => mSut.Go((link, dict) => { })).AndHasMessage($"Must first call {nameof(IScraper.SetTargetPageXPaths)}.");
        }

        /// <summary>
        /// This test will exit after the first result page since the target page will be returned for "page 2".
        /// Since the target page has no result nodes and no next link, the while loop will exit.
        /// </summary>
        [Test]
        public void Go_ShouldScrapeTarget()
        {
            mSut.SetResultsStartPage(ResultsStartPage);
            mSut.SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li");
            mSut.SetIndividualResultLinkXPath("a/@href");
            mSut.SetNextLinkXPath("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href");
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            int called = 0;
            mSut.Go((link, dict) =>
                {
                    called++;
                    StringAssert.StartsWith("https://pullman.craigslist.org/apa/d/", link);
                    Assert.AreEqual(dict["br"], "3BR"); //The same target page is returned for each link, so they should all be the same value
                });

            Assert.AreEqual(120, called); //120 results per page
        }

        /// <summary>
        /// This test will exit after the first result page since the target page will be returned for "page 2".
        /// Since the target page has no result nodes and no next link, the while loop will exit.
        /// </summary>
        [Test]
        public void Go_ShouldSkipNullTargetLink()
        {
            mSut.SetResultsStartPage(ResultsStartPage);
            mSut.SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li");
            mSut.SetIndividualResultLinkXPath("br/@href"); //Doesn't exist - therfore it should return null causing the loop to continue
            mSut.SetNextLinkXPath("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href");
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            int called = 0;
            mSut.Go((link, dict) => { called++; });

            Assert.AreEqual(0, called);
        }

        /// <summary>
        /// This test will exit after the first result page since the target page will be returned for "page 2".
        /// Since the target page has no result nodes and no next link, the while loop will exit.
        /// </summary>
        [Test]
        public void Go_ShouldScrapeTarget_WithPredicate()
        {
            mSut.SetResultsStartPage(ResultsStartPage);
            mSut.SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li");
            mSut.SetIndividualResultLinkXPath("a/@href");
            mSut.SetNextLinkXPath("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href");
            mSut.SetResultVisitPredicate(housing => housing.Contains("3br"), "p/span[2]/span[2]");
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            int called = 0;
            mSut.Go((link, dict) =>
                {
                    called++;
                    StringAssert.StartsWith("https://pullman.craigslist.org/apa/d/", link);
                    Assert.AreEqual(dict["br"], "3BR"); //The same target page is returned for each link, so they should all be the same value
                });

            Assert.AreEqual(45, called); //Only 45 of the 120 are 3br
        }

        /// <summary>
        /// This test will exit after the first result page since the target page will be returned for "page 2".
        /// Since the target page has no result nodes and no next link, the while loop will exit.
        /// </summary>
        [Test]
        public void Go_ShouldScrapeTarget_WithPredicateXPathThatReturnsNull()
        {
            mSut.SetResultsStartPage(ResultsStartPage);
            mSut.SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li");
            mSut.SetIndividualResultLinkXPath("a/@href");
            mSut.SetNextLinkXPath("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href");
            mSut.SetResultVisitPredicate(housing => housing == null, "p/span[2]/span[2]/li"); //Doesn't exist - which should pass null to predicate
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            int called = 0;
            mSut.Go((link, dict) =>
                {
                    called++;
                    StringAssert.StartsWith("https://pullman.craigslist.org/apa/d/", link);
                    Assert.AreEqual(dict["br"], "3BR"); //The same target page is returned for each link, so they should all be the same value
                });

            Assert.AreEqual(120, called); //120 results per page
        }

        /// <summary>
        /// This test will exit after the first result page since the target page will be returned for "page 2".
        /// Since the target page has no result nodes and no next link, the while loop will exit.
        /// </summary>
        [Test]
        public void Go_ShouldScrapeTarget_WithPredicateAndThrottles()
        {
            mSut.SetResultsStartPage(ResultsStartPage);
            mSut.SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li");
            mSut.SetIndividualResultLinkXPath("a/@href");
            mSut.SetNextLinkXPath("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href");
            mSut.SetResultVisitPredicate(housing => housing.Contains("3br"), "p/span[2]/span[2]");
            mSut.ThrottleSearchResultRetrieval(TimeSpan.FromMilliseconds(1));
            mSut.ThrottleTargetResultRetrieval(TimeSpan.FromMilliseconds(1));
            mSut.SetTargetPageXPaths(new Dictionary<string, string> { { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" } });

            int called = 0;
            mSut.Go((link, dict) =>
                {
                    called++;
                    StringAssert.StartsWith("https://pullman.craigslist.org/apa/d/", link);
                    Assert.AreEqual(dict["br"], "3BR"); //The same target page is returned for each link, so they should all be the same value
                });

            Assert.AreEqual(45, called); //Only 45 of the 120 are 3br
        }
    }
}
