// Copyright © 2018 Alex Leendertsen

using ScrapeX.Interfaces;

namespace ScrapeX
{
    public class ScraperFactory : IScraperFactory
    {
        private readonly INavigatorFactory mNavigatorFactory = new NavigatorFactory();

        public IScraper CreateSinglePageScraper(string baseUrl)
        {
            return new Scraper(baseUrl, mNavigatorFactory);
        }

        public IPaginatingScraper CreatePaginatingScraper(string baseUrl)
        {
            return new PaginatingScraper(baseUrl, mNavigatorFactory);
        }
    }
}
