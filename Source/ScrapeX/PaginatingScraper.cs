// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.XPath;

[assembly: InternalsVisibleTo("ScrapeX.Test")]

namespace ScrapeX
{
    //TODO validate minimal configuration and throw on Go() if null
    //TODO Handle unspecified optional parameters (e.g. predicate)
    internal class PaginatingScraper : Scraper, IPaginatingScraper
    {
        private string mResultsStartPageUrl;
        private XPathExpression mNextLinkXPath;
        private XPathExpression mIndividualNodeXPath;
        private XPathExpression mIndividualLinkXPath;
        private TimeSpan mThrottle;
        private Predicate<string> mShouldVisitResult;
        private XPathExpression mPredicateXPath;

        internal PaginatingScraper(string baseUrl)
            : base(baseUrl)
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

        public IPaginatingScraper SetNextLink(string xPath)
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
            mThrottle = timeSpan;
            return this;
        }

        public IPaginatingScraper ThrottleSearchResultRetrieval(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
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

                    ScrapeTarget(link, onTargetRetrieved);
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
                throw new InvalidCastException($"Must first call {nameof(SetResultsStartPage)}.");
            }

            base.ValidateMinimalOptions();
        }
    }
}
