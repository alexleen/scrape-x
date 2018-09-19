// Copyright © 2018 Alex Leendertsen

using System;
using NUnit.Framework;

namespace ScrapeX.Test
{
    [TestFixture]
    public class ScraperTest
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Ctor_ShouldThrowIfInvalidBaseUrl(string invalidUrl)
        {
            Assert.Throws<ArgumentException>(() => new Scraper(invalidUrl));
        }
    }
}
