using NUnit.Framework;

namespace ScrapeX.Test
{
    [TestFixture]
    public class ScraperFactoryTest
    {
        private ScraperFactory mSut;

        [SetUp]
        public void SetUp()
        {
            mSut = new ScraperFactory();
        }

        [Test]
        public void CreateSinglePageScraper_ShouldReturnScraper()
        {
            Assert.IsInstanceOf<Scraper>(mSut.CreateSinglePageScraper("url"));
        }

        [Test]
        public void CreatePaginatingScraper_ShouldReturnScraper()
        {
            Assert.IsInstanceOf<PaginatingScraper>(mSut.CreatePaginatingScraper("url"));
        }
    }
}
