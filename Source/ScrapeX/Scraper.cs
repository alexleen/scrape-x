// Copyright © 2018 Alex Leendertsen

using HtmlAgilityPack;
using ScrapeX.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.XPath;

namespace ScrapeX
{
    internal class Scraper : IScraper
    {
        protected readonly string BaseUrl;
        private readonly INavigatorFactory mNavigatorFactory;
        private readonly HtmlWeb mHtmlWeb;

        private HttpClient mHttpClient;
        private IDictionary<string, string> mXPaths;
        private IDictionary<string, IEnumerable<string>> mTableCellXPaths;

        internal Scraper(string baseUrl, INavigatorFactory navigatorFactory)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ArgumentException("message", nameof(baseUrl));
            }

            BaseUrl = baseUrl;
            mNavigatorFactory = navigatorFactory;
            mHtmlWeb = new HtmlWeb();
        }

        public IScraper UseHttpClient(HttpClient httpClient)
        {
            mHttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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

        public IScraper SetTableXPaths(IDictionary<string, IEnumerable<string>> tableCellXPaths)
        {
            if (tableCellXPaths == null)
            {
                throw new ArgumentNullException(nameof(tableCellXPaths));
            }

            if (tableCellXPaths.Count == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(tableCellXPaths));
            }

            mTableCellXPaths = tableCellXPaths;
            return this;
        }

        public virtual void Go(Action<string, IDictionary<string, string>, IDictionary<string, IEnumerable<IEnumerable<string>>>> onTargetRetrieved)
        {
            if (onTargetRetrieved == null)
            {
                throw new ArgumentNullException(nameof(onTargetRetrieved));
            }

            ValidateMinimalOptions();

            ScrapeTarget(BaseUrl, onTargetRetrieved);
        }

        /// <summary>
        /// Retrieves the specified link, scrapes it, and invokes <paramref name="onTargetRetrieved"/> with the results.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="onTargetRetrieved"></param>
        protected void ScrapeTarget(string link, Action<string, IDictionary<string, string>, IDictionary<string, IEnumerable<IEnumerable<string>>>> onTargetRetrieved)
        {
            XPathNavigator listing = Get(link);
            Scrape(listing, mXPaths, link, onTargetRetrieved);
        }

        /// <summary>
        /// Scrapes the specified <paramref name="navigator"/> using the specified <paramref name="xPaths"/> and invokes <paramref name="onTargetRetrieved"/> with the results.
        /// <paramref name="link"/> is only used for callback. <paramref name="link"/> is NOT retrieved.
        /// </summary>
        /// <param name="navigator"></param>
        /// <param name="xPaths"></param>
        /// <param name="link"></param>
        /// <param name="onTargetRetrieved"></param>
        protected void Scrape(XPathNavigator navigator, IDictionary<string, string> xPaths, string link, Action<string, IDictionary<string, string>, IDictionary<string, IEnumerable<IEnumerable<string>>>> onTargetRetrieved)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            if (xPaths != null)
            {
                foreach (KeyValuePair<string, string> kvp in xPaths)
                {
                    results[kvp.Key] = navigator.SelectSingleNode(XPathExpression.Compile(kvp.Value))?.Value;
                }
            }

            IDictionary<string, IEnumerable<IEnumerable<string>>> tables = null;

            if (mTableCellXPaths != null)
            {
                tables = ScrapeTables();
            }

            onTargetRetrieved(link, results, tables);
        }

        private IDictionary<string, IEnumerable<IEnumerable<string>>> ScrapeTables()
        {
            IDictionary<string, IEnumerable<IEnumerable<string>>> result = new Dictionary<string, IEnumerable<IEnumerable<string>>>();

            XPathNavigator listing = Get(BaseUrl);

            foreach (KeyValuePair<string, IEnumerable<string>> kvp in mTableCellXPaths)
            {
                IList<IList<string>> colValues = new List<IList<string>>();
                int currCol = 0;

                foreach (string cellXPath in kvp.Value)
                {
                    if (colValues.Count <= currCol)
                    {
                        colValues.Add(new List<string>());
                    }

                    foreach (XPathNavigator cellNav in listing.Select(XPathExpression.Compile(cellXPath)))
                    {
                        colValues[currCol].Add(cellNav.Value);
                    }

                    currCol++;
                }

                result.Add(kvp.Key, colValues);
            }

            return result;
        }

        protected virtual void ValidateMinimalOptions()
        {
            if (mXPaths == null && mTableCellXPaths == null)
            {
                throw new InvalidOperationException($"Must first call either {nameof(SetTargetPageXPaths)} or {nameof(SetTableXPaths)}.");
            }
        }

        protected XPathNavigator Get(string url)
        {
            return mNavigatorFactory.Create(url, mHttpClient, mHtmlWeb);
        }

        /// <summary>
        /// Whether or not <see cref="Scraper"/> is setup to scrape a target result page (<see cref="mXPaths"/> is non-null).
        /// </summary>
        protected bool IsSetupToScrapeTarget => mXPaths != null;
    }
}
