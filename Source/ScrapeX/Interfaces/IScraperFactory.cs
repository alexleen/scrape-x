// Copyright © 2018 Alex Leendertsen

namespace ScrapeX.Interfaces
{
    /// <summary>
    /// Creates various types of scrapers.
    /// </summary>
    public interface IScraperFactory
    {
        /// <summary>
        /// Creates a single page scraper for the specified <paramref name="url"/>.        
        /// </summary>
        /// <param name="url">URL of the page to be scraped</param>
        /// <returns>An instance of a single page scraper.</returns>
        /// <remarks>
        /// To scrape multiple pages, use <see cref="CreatePaginatingScraper(string)"/>.
        /// </remarks>
        IScraper CreateSinglePageScraper(string url);

        /// <summary>
        /// Creates a scraper to handle pages of results.        
        /// </summary>
        /// <param name="baseUrl">Base URL for the web site to the scraped.</param>
        /// <returns>An instance of a paginating scraper.</returns>
        /// <remarks>
        /// See <see cref="IPaginatingScraper"/> on how to setup the scraper's result page,
        /// next link, and individual result node. To scrape a single page, use <see cref="CreateSinglePageScraper(string)"/>.
        /// </remarks>
        IPaginatingScraper CreatePaginatingScraper(string baseUrl);
    }
}
