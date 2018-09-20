// Copyright © 2018 Alex Leendertsen

namespace ScrapeX
{
    public interface IScraperFactory
    {
        /// <summary>
        /// Creates a scraper for the specified <paramref name="baseUrl"/>.
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        IScraper Create(string baseUrl);
    }
}
