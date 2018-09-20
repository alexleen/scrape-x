// Copyright © 2018 Alex Leendertsen

namespace ScrapeX
{
    public class ScraperFactory : IScraperFactory
    {
        public IScraper CreateSinglePageScraper(string baseUrl)
        {
            return new Scraper(baseUrl);
        }

        public IPaginatingScraper CreatePaginatingScraper(string baseUrl)
        {
            return new PaginatingScraper(baseUrl);
        }
    }
}
