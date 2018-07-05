// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace ScrapeX
{
    public class Scraper
    {
        private readonly string mBaseUrl;
        private string mResultsStartPageUrl;
        private string mNextLinkXPath;
        private IDictionary<string, string> mXPaths;
        private string mIndividualNodeXPath;
        private string mIndividualLinkXPath;
        private TimeSpan mThrottle;
        private Predicate<string> mShouldVisitResult;
        private string mPredicateXPath;

        public Scraper(string baseUrl)
        {
            mBaseUrl = baseUrl;
        }

        /// <summary>
        /// Sets the URL for the first page of search results.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Scraper SetResultsStartPage(string url)
        {
            mResultsStartPageUrl = url;
            return this;
        }

        /// <summary>
        /// Sets the XPath for the link to the next page of search results.
        /// </summary>
        /// <param name="nextLinkXPath"></param>
        /// <returns></returns>
        public Scraper SetNextLink(string nextLinkXPath)
        {
            mNextLinkXPath = nextLinkXPath;
            return this;
        }

        /// <summary>
        /// Sets the XPath for the node of each search result.
        /// Node will be used when retrieving the link to the result (specified in <see cref="SetIndividualResultLinkXPath"/>)
        /// and when evaulating the result predicate (specified in <see cref="SetResultVisitPredicate"/>).
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public Scraper SetIndividualResultNodeXPath(string xPath)
        {
            mIndividualNodeXPath = xPath;
            return this;
        }

        /// <summary>
        /// XPath to the link for the search result relative to the individual result node specified in <see cref="SetIndividualResultNodeXPath"/>.
        /// </summary>
        /// <param name="xPath"></param>
        /// <returns></returns>
        public Scraper SetIndividualResultLinkXPath(string xPath)
        {
            mIndividualLinkXPath = xPath;
            return this;
        }

        /// <summary>
        /// Sets a predicate that is evaluated before visiting a search result.
        /// Only if the result returns true is the result visited.        
        /// </summary>
        /// <param name="shouldVisitResult"></param>
        /// <param name="xPath">XPath relative to search result node</param>
        /// <returns></returns>
        public Scraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath)
        {
            mShouldVisitResult = shouldVisitResult;
            mPredicateXPath = xPath;
            return this;
        }

        /// <summary>
        /// Sets the keys and associated XPaths for retrieving data from the target page.
        /// Keys are used to identify the individual data points in the callback to the <see cref="Go"/> method.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        public Scraper SetTargetPageXPaths(IDictionary<string, string> xPaths)
        {
            mXPaths = xPaths;
            return this;
        }

        /// <summary>
        /// Specifies an amount of time to wait before retrieving each search result.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public Scraper ThrottleTargetResultRetrieval(TimeSpan timeSpan)
        {
            mThrottle = timeSpan;
            return this;
        }

        /// <summary>
        /// Specifies an amount of time to wait before retrieving a new page of search results.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public Scraper ThrottleSearchResultRetrieval(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public void Go(Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            HtmlWeb web = new HtmlWeb();
            string currentResultsPageUrl = mResultsStartPageUrl;

            do
            {
                string currentPage = mBaseUrl + currentResultsPageUrl;

                XPathNavigator searchPage = web.Load(currentPage).CreateNavigator();

                XPathNodeIterator searchResultNodes = searchPage.Select(mIndividualNodeXPath);

                foreach (XPathNavigator result in searchResultNodes)
                {
                    if (!mShouldVisitResult(result.SelectSingleNode(mPredicateXPath)?.Value))
                    {
                        continue;
                    }

                    string link = result.SelectSingleNode(mIndividualLinkXPath)?.Value;

                    if (link == null)
                    {
                        continue;
                    }

                    if (mThrottle != default(TimeSpan))
                    {
                        Thread.Sleep(mThrottle);
                    }

                    XPathNavigator listing = web.Load(link).CreateNavigator();
                    IDictionary<string, string> results = new Dictionary<string, string>();

                    foreach (KeyValuePair<string, string> kvp in mXPaths)
                    {
                        results[kvp.Key] = listing.SelectSingleNode(kvp.Value)?.Value;
                    }

                    onTargetRetrieved(link, results);
                }

                //next link's href. Won't exist on last page.
                currentResultsPageUrl = searchPage.SelectSingleNode(mNextLinkXPath)?.Value;
            }
            while (!string.IsNullOrEmpty(currentResultsPageUrl));
        }
    }
}
