// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Xml.XPath;
using NUnit.Framework;

namespace ScrapeX.Test
{
    [TestFixture]
    public class ScraperTest
    {
        private IPaginatingScraper mSut;

        [SetUp]
        public void SetUp()
        {
            mSut = new PaginatingScraper("url");
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
            Assert.Throws<ArgumentException>(() => new PaginatingScraper(invalidUrl));
        }

        [Test]
        public void UseHttpClient_ShouldThrow_WhenNull()
        {
            Assert.Throws<ArgumentNullException>(() => mSut.UseHttpClient(null));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetResultsStartPage_ShouldThrow_WhenInvalidUrl(string invalidUrl)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetResultsStartPage(invalidUrl));
        }

        [TestCaseSource(nameof(sInvalidStrings))]
        public void SetNextLink_ShouldThrow_WhenInvalidString(string invalidXPath)
        {
            Assert.Throws<ArgumentException>(() => mSut.SetNextLink(invalidXPath));
        }

        [TestCaseSource(nameof(sInvalidXPaths))]
        public void SetNextLink_ShouldThrow_WhenInvalidXPath(string invalidXPath)
        {
            Assert.Throws<XPathException>(() => mSut.SetNextLink(invalidXPath));
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
    }
}
