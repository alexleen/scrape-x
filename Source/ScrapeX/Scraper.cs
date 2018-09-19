// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.XPath;
using HtmlAgilityPack;

[assembly: InternalsVisibleTo("ScrapeX.Test")]

namespace ScrapeX
{
    //TODO validate minimal configuration and throw on Go() if null
    //TODO Handle unspecified optional parameters (e.g. predicate)
    //TODO strategies - which will be how the above two TODOs will be accomplished
    //TODO compile XPaths and store as XPathExpressions? Or just take XPathExpressions as params? Overloads? https://stackoverflow.com/questions/308926/verify-an-xpath-in-net
    internal class Scraper : IScraper
    {
        private readonly string mBaseUrl;
        private readonly HtmlWeb mHtmlWeb;

        private HttpClient mHttpClient;
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
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("message", nameof(baseUrl));
            }

            mBaseUrl = baseUrl;
            mHtmlWeb = new HtmlWeb();
        }

        public IScraper UseHttpClient(HttpClient httpClient)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            return this;
        }

        public IScraper SetResultsStartPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));
            }

            mResultsStartPageUrl = url;
            return this;
        }

        public IScraper SetNextLink(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mNextLinkXPath = xPath;
            return this;
        }

        public IScraper SetIndividualResultNodeXPath(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mIndividualNodeXPath = xPath;
            return this;
        }

        public IScraper SetIndividualResultLinkXPath(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mIndividualLinkXPath = xPath;
            return this;
        }

        public IScraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mShouldVisitResult = shouldVisitResult ?? throw new ArgumentNullException(nameof(shouldVisitResult));
            mPredicateXPath = xPath;
            return this;
        }

        public IScraper SetTargetPageXPaths(IDictionary<string, string> xPaths)
        {
            if (xPaths == null)
            {
                throw new ArgumentNullException(nameof(xPaths));
            }

            if (xPaths.Count == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(xPaths));
            }

            mXPaths = xPaths;
            return this;
        }

        public IScraper ThrottleTargetResultRetrieval(TimeSpan timeSpan)
        {
            mThrottle = timeSpan;
            return this;
        }

        public IScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        //TODO async version?
        public void Go(Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            if (onTargetRetrieved == null)
            {
                throw new ArgumentNullException(nameof(onTargetRetrieved));
            }

            ValidateMinimalOptions();

            string currentResultsPageUrl = mResultsStartPageUrl;

            do
            {
                string currentPage = mBaseUrl + currentResultsPageUrl;

                XPathNavigator searchPage = Get(currentPage);

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

                    XPathNavigator listing = Get(link);
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

        private void ValidateMinimalOptions()
        {
            if (mXPaths == null)
            {
                throw new InvalidOperationException($"Must first call {nameof(SetTargetPageXPaths)}.");
            }
        }

        private XPathNavigator Get(string url)
        {
            if (mHttpClient == null)
            {
                return mHtmlWeb.Load(url).CreateNavigator();
            }

            HttpResponseMessage response = mHttpClient.GetAsync(url).Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);
            return htmlDoc.CreateNavigator();
        }
    }
}
