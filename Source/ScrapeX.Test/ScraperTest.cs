using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace ScrapeX.Test
{
    [TestFixture]
    public class ScraperTest
    {
        private IScraper mSut;

        [SetUp]
        public void SetUp()
        {
            mSut = new Scraper("baseUrl", null);
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
    }
}
