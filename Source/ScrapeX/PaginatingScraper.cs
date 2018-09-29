// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.XPath;
using ScrapeX.Interfaces;

[assembly: InternalsVisibleTo("ScrapeX.Test")]

namespace ScrapeX
{
    internal class PaginatingScraper : Scraper, IPaginatingScraper
    {
        private string mResultsStartPageUrl;
        private XPathExpression mNextLinkXPath;
        private XPathExpression mIndividualNodeXPath;
        private XPathExpression mIndividualLinkXPath;
        private TimeSpan mResultRetrievalThrottle;
        private TimeSpan mPageRetrievalThrottle;
        private Predicate<string> mShouldVisitResult;
        private XPathExpression mPredicateXPath;
        private IDictionary<string, string> mXPaths;

        internal PaginatingScraper(string baseUrl, INavigatorFactory navigatorFactory)
            : base(baseUrl, navigatorFactory)
        {
        }

        public IPaginatingScraper SetResultsStartPage(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(url));
            }

            mResultsStartPageUrl = url;
            return this;
        }

        public IPaginatingScraper SetNextLinkXPath(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mNextLinkXPath = XPathExpression.Compile(xPath);
            return this;
        }

        public IPaginatingScraper SetIndividualResultNodeXPath(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mIndividualNodeXPath = XPathExpression.Compile(xPath);
            return this;
        }

        public IPaginatingScraper SetIndividualResultLinkXPath(string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mIndividualLinkXPath = XPathExpression.Compile(xPath);
            return this;
        }

        public IPaginatingScraper SetResultVisitPredicate(Predicate<string> shouldVisitResult, string xPath)
        {
            if (string.IsNullOrWhiteSpace(xPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(xPath));
            }

            mShouldVisitResult = shouldVisitResult ?? throw new ArgumentNullException(nameof(shouldVisitResult));
            mPredicateXPath = XPathExpression.Compile(xPath);
            return this;
        }

        public IPaginatingScraper ThrottleTargetResultRetrieval(TimeSpan timeSpan)
        {
            mResultRetrievalThrottle = timeSpan;
            return this;
        }

        public IPaginatingScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan)
        {
            mPageRetrievalThrottle = timeSpan;
            return this;
        }

        public IPaginatingScraper SetResultPageXPaths(IDictionary<string, string> xPaths)
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

        public override void Go(Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            if (onTargetRetrieved == null)
            {
                throw new ArgumentNullException(nameof(onTargetRetrieved));
            }

            ValidateMinimalOptions();

            string currentResultsPageUrl = mResultsStartPageUrl;

            do
            {
                string currentPage = BaseUrl + currentResultsPageUrl;

                //TimeSpans are zero by default, so if mThrottle isn't set this doesn't sleep
                Thread.Sleep(mPageRetrievalThrottle);

                XPathNavigator searchPage = Get(currentPage);

                XPathNodeIterator searchResultNodes = searchPage.Select(mIndividualNodeXPath);

                foreach (XPathNavigator result in searchResultNodes)
                {
                    if (mShouldVisitResult != null && !mShouldVisitResult(result.SelectSingleNode(mPredicateXPath)?.Value))
                    {
                        continue;
                    }

                    ScrapeResultPage(onTargetRetrieved, currentPage, result);

                    ScrapeTargetPage(result, onTargetRetrieved);
                }

                //next link's href. Won't exist on last page.
                currentResultsPageUrl = searchPage.SelectSingleNode(mNextLinkXPath)?.Value;
            }
            while (!string.IsNullOrEmpty(currentResultsPageUrl));
        }

        protected override void ValidateMinimalOptions()
        {
            if (mResultsStartPageUrl == null)
            {
                throw new InvalidOperationException($"Must first call {nameof(SetResultsStartPage)}.");
            }

            if (mIndividualNodeXPath == null)
            {
                throw new InvalidOperationException($"Must first call {nameof(SetIndividualResultNodeXPath)}.");
            }

            if (mNextLinkXPath == null)
            {
                throw new InvalidOperationException($"Must first call {nameof(SetNextLinkXPath)}.");
            }

            if (IsSetupToScrapeTarget)
            {
                if (mIndividualLinkXPath == null)
                {
                    throw new InvalidOperationException($"Must first call {nameof(SetIndividualResultLinkXPath)}.");
                }

                base.ValidateMinimalOptions();
            }
            else
            {
                if (mXPaths == null)
                {
                    throw new InvalidOperationException($"Must first call either {nameof(IScraper.SetTargetPageXPaths)} or {nameof(IPaginatingScraper.SetResultPageXPaths)} in order to scrape data.");
                }

                if (mIndividualLinkXPath != null)
                {
                    throw new InvalidOperationException($"Possible misconfiguration: {nameof(IPaginatingScraper.SetIndividualResultLinkXPath)} should not be called when not scraping target result pages because it has no effect.");
                }
            }
        }

        /// <summary>
        /// Scrapes result page, if configured.
        /// </summary>
        /// <param name="onTargetRetrieved"></param>
        /// <param name="currentPage"></param>
        /// <param name="navigator"></param>
        private void ScrapeResultPage(Action<string, IDictionary<string, string>> onTargetRetrieved, string currentPage, XPathNavigator navigator)
        {
            if (mXPaths != null)
            {
                Scrape(currentPage, navigator, mXPaths, onTargetRetrieved);
            }
        }

        /// <summary>
        /// Scrapes target page, if configured.
        /// </summary>
        /// <param name="navigator"></param>
        /// <param name="onTargetRetrieved"></param>
        private void ScrapeTargetPage(XPathNavigator navigator, Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            if (IsSetupToScrapeTarget)
            {
                string link = navigator.SelectSingleNode(mIndividualLinkXPath)?.Value;

                if (link == null)
                {
                    return;
                }

                //TimeSpans are zero by default, so if mThrottle isn't set this doesn't sleep
                Thread.Sleep(mResultRetrievalThrottle);

                ScrapeTarget(link, onTargetRetrieved);
            }
        }
    }
}
