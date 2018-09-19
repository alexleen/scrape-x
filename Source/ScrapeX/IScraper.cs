﻿// Copyright © 2018 Alex Leendertsen

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
        /// Sets the URL for the first page of search results.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty, or consists only of whitespace characters.</exception>
        IScraper SetResultsStartPage(string url);

        /// <summary>
        /// Sets the XPath for the link to the next page of search results.
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, or consists only of whitespace characters.</exception>
        IScraper SetNextLink(string xPath);

        /// <summary>
        /// Sets the XPath for the node of each search result.
        /// Node will be used when retrieving the link to the result (specified in <see cref="SetIndividualResultLinkXPath"/>)
        /// and when evaluating the result predicate (specified in <see cref="SetResultVisitPredicate"/>).
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, or consists only of whitespace characters.</exception>
        IScraper SetIndividualResultNodeXPath(string xPath);

        /// <summary>
        /// XPath to the link for the search result relative to the individual result node specified in <see cref="SetIndividualResultNodeXPath"/>.
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, or consists only of whitespace characters.</exception>
        IScraper SetIndividualResultLinkXPath(string xPath);

        /// <summary>
        /// Sets a predicate that is evaluated before visiting a search result.
        /// Only if the result returns true is the result visited.        
        /// </summary>
        /// <param name="shouldVisitResult"></param>
        /// <param name="xPath">XPath relative to search result node</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="shouldVisitResult"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, or consists only of whitespace characters.</exception>
        IScraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath);

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
        /// Specifies an amount of time to wait before retrieving each search result.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        IScraper ThrottleTargetResultRetrieval(TimeSpan timeSpan);

        /// <summary>
        /// Specifies an amount of time to wait before retrieving a new page of search results.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        IScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan);

        /// <summary>
        /// Begins synchronously scraping. Will call <paramref name="onTargetRetrieved"/> for each scraped target page.
        /// </summary>
        /// <param name="onTargetRetrieved"></param>
        /// <exception cref="ArgumentNullException"><paramref name="onTargetRetrieved"/> is null.</exception>
        void Go(Action<string, IDictionary<string, string>> onTargetRetrieved);
    }
}
