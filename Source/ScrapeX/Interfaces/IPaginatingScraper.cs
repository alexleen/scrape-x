// Copyright © 2018 Alex Leendertsen

using System;
using System.Xml.XPath;
using System.Collections.Generic;

namespace ScrapeX.Interfaces
{
    /// <summary>
    /// Scraper that iterates through pages of results.
    /// </summary>
    public interface IPaginatingScraper : IScraper
    {
        /// <summary>
        /// Sets the URL for the first page of search results.
        /// </summary>
        /// <param name="url">Url of first result page relative to the URL specified in <see cref="IScraperFactory.CreatePaginatingScraper(string)"/>.</param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty, or consists only of whitespace characters.</exception>
        /// <remarks>
        /// This is where the scraper will start gathering links specified by <see cref="SetIndividualResultLinkXPath(string)"/>.
        /// Subsequent pages will be visited by navigating to the "next link" specified by <see cref="SetNextLinkXPath(string)"/>.
        /// </remarks>
        IPaginatingScraper SetResultsStartPage(string url);

        /// <summary>
        /// Sets the XPath for the link to the next page of search results.
        /// </summary>
        /// <param name="xPath">XPath for the string URL of the next page.</param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        /// <remarks>
        /// Value returned by XPath must a the string URL of the next page of results.
        /// XPath should return null on the last page of results, which signals the scraper to exit.
        /// </remarks>
        IPaginatingScraper SetNextLinkXPath(string xPath);

        /// <summary>
        /// Sets the XPath for the node of each search result.        
        /// </summary>
        /// <param name="xPath">XPath for the node that represents a search result.</param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        /// <remarks>
        /// XPath should return a single node for each search result suitable for link retrieval via <see cref="SetIndividualResultLinkXPath"/>,
        /// predicate evaluation via <see cref="SetResultVisitPredicate"/>, and scraping via <see cref="SetResultPageXPaths(IDictionary{string, string})"/>.
        /// </remarks>
        IPaginatingScraper SetIndividualResultNodeXPath(string xPath);

        /// <summary>
        /// XPath to the link for the search result. Relative to the individual result node specified in <see cref="SetIndividualResultNodeXPath"/>.
        /// </summary>
        /// <param name="xPath">XPath for the string URL of the result link relative to the result node.</param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        /// <remarks>
        /// This is the link to a target page that will be scraped. 
        /// Results will be delivered via the callback of the <see cref="IScraper.Go(Action{string, IDictionary{string, string}})"/> method.
        /// </remarks>
        IPaginatingScraper SetIndividualResultLinkXPath(string xPath);

        /// <summary>
        /// Sets a predicate that is evaluated before visiting a search result.
        /// Only if the predicate returns true is the result visited. Optional.
        /// </summary>
        /// <param name="shouldVisitResult">Predicate that evaluates the value returned by <paramref name="xPath"/>.</param>
        /// <param name="xPath">XPath for the predicate value relative to the result node.</param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="shouldVisitResult"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        /// <remarks>
        /// This allows for considerable speed improvements as well as bandwidth conservation as it allows the scraper to skip results that may not be relevant.
        /// </remarks>
        IPaginatingScraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath);

        /// <summary>
        /// Specifies an amount of time to wait before retrieving each search result. Optional.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        IPaginatingScraper ThrottleTargetResultRetrieval(TimeSpan timeSpan);

        /// <summary>
        /// Specifies an amount of time to wait before retrieving a new page of search results. Optional.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns>This instance of <see cref="IPaginatingScraper"/>.</returns>
        IPaginatingScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan);

        /// <summary>
        /// Sets the keys and associated XPaths for retrieving data from the result page.
        /// Keys are used to identify the individual data points in the callback to the <see cref="IScraper.Go(Action{string, IDictionary{string, string}})"/> method.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <returns>This instance of <see cref="IScraper"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="xPaths"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPaths"/> is empty.</exception>
        /// <remarks>
        /// Either this method or <see cref="IScraper.SetTargetPageXPaths(IDictionary{string, string})"/> must be called to scrape some data.
        /// </remarks>
        IPaginatingScraper SetResultPageXPaths(IDictionary<string, string> xPaths);
    }
}
