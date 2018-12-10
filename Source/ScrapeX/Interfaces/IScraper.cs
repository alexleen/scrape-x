// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ScrapeX.Interfaces
{
    /// <summary>
    /// Scraper that scrapes a single web page.
    /// </summary>
    public interface IScraper
    {


        /// <summary>
        /// Use this HttpClient instead of the built in <see cref="HtmlWeb"/>.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns>This instance of <see cref="IScraper"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        IScraper UseHttpClient(HttpClient httpClient);

        /// <summary>
        /// Sets the keys and associated XPaths for retrieving data from the target page.
        /// Keys are used to identify the individual data points in the callback to the <see cref="Go"/> method.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <returns>This instance of <see cref="IScraper"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="xPaths"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPaths"/> is empty.</exception>
        /// <remarks>
        /// Either this method or <see cref="IPaginatingScraper.SetResultPageXPaths(IDictionary{string, string})"/> must be called to scrape some data.
        /// </remarks>
        IScraper SetTargetPageXPaths(IDictionary<string, string> xPaths);

        IScraper SetTableXPaths(IDictionary<string, IEnumerable<string>> tableRowXPaths);

        /// <summary>
        /// Begins synchronously scraping. Will call <paramref name="onTargetRetrieved"/> for each scraped target page.
        /// </summary>
        /// <param name="onTargetRetrieved">Callback with a link as well as keys and their corresponding values as defined by 
        /// <see cref="SetTargetPageXPaths(IDictionary{string, string})"/> and/or <see cref="IPaginatingScraper.SetResultPageXPaths(IDictionary{string, string})"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="onTargetRetrieved"/> is null.</exception>
        void Go(Action<string, IDictionary<string, string>> onTargetRetrieved);

        void GoTables(Action<string, IDictionary<string, IList<IList<string>>>> onTablesRetrieved);
    }
}
