// Copyright © 2018 Alex Leendertsen

using ScrapeX.Interfaces;

namespace ScrapeX
{
    public class ScraperFactory : IScraperFactory
    {
        private readonly INavigatorFactory mNavigatorFactory = new NavigatorFactory();

        public IScraper CreateSinglePageScraper(string url)
        {
            return new Scraper(url, mNavigatorFactory);
        }

        public IPaginatingScraper CreatePaginatingScraper(string baseUrl)
        {
            return new PaginatingScraper(baseUrl, mNavigatorFactory);
        }
    }
}
