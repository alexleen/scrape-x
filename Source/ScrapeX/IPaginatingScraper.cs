// Copyright © 2018 Alex Leendertsen

using System;
using System.Xml.XPath;

namespace ScrapeX
{
    public interface IPaginatingScraper : IScraper
    {
        /// <summary>
        /// Sets the URL for the first page of search results.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty, or consists only of whitespace characters.</exception>
        IPaginatingScraper SetResultsStartPage(string url);

        /// <summary>
        /// Sets the XPath for the link to the next page of search results.
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        IPaginatingScraper SetNextLink(string xPath);

        /// <summary>
        /// Sets the XPath for the node of each search result.
        /// Node will be used when retrieving the link to the result (specified in <see cref="SetIndividualResultLinkXPath"/>)
        /// and when evaluating the result predicate (specified in <see cref="SetResultVisitPredicate"/>).
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        IPaginatingScraper SetIndividualResultNodeXPath(string xPath);

        /// <summary>
        /// XPath to the link for the search result relative to the individual result node specified in <see cref="SetIndividualResultNodeXPath"/>.
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        IPaginatingScraper SetIndividualResultLinkXPath(string xPath);

        /// <summary>
        /// Sets a predicate that is evaluated before visiting a search result.
        /// Only if the result returns true is the result visited.        
        /// </summary>
        /// <param name="shouldVisitResult"></param>
        /// <param name="xPath">XPath relative to search result node</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="shouldVisitResult"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xPath"/> is null, empty, consists only of whitespace characters, or is not a valid XPath expression.</exception>
        /// <exception cref="XPathException"><paramref name="xPath"/> is not valid.</exception>
        IPaginatingScraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath);

        /// <summary>
        /// Specifies an amount of time to wait before retrieving each search result.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        IPaginatingScraper ThrottleTargetResultRetrieval(TimeSpan timeSpan);

        /// <summary>
        /// Specifies an amount of time to wait before retrieving a new page of search results.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        IPaginatingScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan);
    }
}
