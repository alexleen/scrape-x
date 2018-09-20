// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Net.Http;

namespace ScrapeX
{
    public interface IScraper
    {
        /// <summary>
        /// Use this HttpClient instead of the built in method.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="httpClient"/> is null.</exception>
        IScraper UseHttpClient(HttpClient httpClient);

        /// <summary>
        /// Sets the keys and associated XPaths for retrieving data from the target page.
        /// Keys are used to identify the individual data points in the callback to the <see cref="Go"/> method.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="xPaths"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPaths"/> is empty.</exception>
        IScraper SetTargetPageXPaths(IDictionary<string, string> xPaths);

        /// <summary>
        /// Begins synchronously scraping. Will call <paramref name="onTargetRetrieved"/> for each scraped target page.
        /// </summary>
        /// <param name="onTargetRetrieved"></param>
        /// <exception cref="ArgumentNullException"><paramref name="onTargetRetrieved"/> is null.</exception>
        void Go(Action<string, IDictionary<string, string>> onTargetRetrieved);
    }
}
