// Copyright © 2018 Alex Leendertsen

namespace ScrapeX
{
    public class ScraperFactory : IScraperFactory
    {
        public IScraper Create(string baseUrl)
        {
            return new Scraper(baseUrl);
        }
    }
}
