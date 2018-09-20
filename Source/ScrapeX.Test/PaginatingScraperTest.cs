// Copyright © 2018 Alex Leendertsen

using NSubstitute;
using NUnit.Framework;
using ScrapeX.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace ScrapeX.Test
{
    [TestFixture]
    public class PaginatingScraperTest
    {
        private IPaginatingScraper mSut;

        [SetUp]
        public void SetUp()
        {
            mSut = new PaginatingScraper("url", Substitute.For<INavigatorFactory>());
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
            Assert.Throws<ArgumentException>(() => new PaginatingScraper(invalidUrl, null));
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
    }
}
