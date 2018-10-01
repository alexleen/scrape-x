// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.XPath;
using HtmlAgilityPack;
using ScrapeX.Interfaces;

namespace ScrapeX
{
    internal class Scraper : IScraper
    {
        protected readonly string BaseUrl;
        private readonly INavigatorFactory mNavigatorFactory;
        private readonly HtmlWeb mHtmlWeb;

        private HttpClient mHttpClient;
        private IDictionary<string, string> mXPaths;

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

        public virtual void Go(Action<string, IDictionary<string, string>> onTargetRetrieved)
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
        protected void ScrapeTarget(string link, Action<string, IDictionary<string, string>> onTargetRetrieved)
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
        protected static void Scrape(XPathNavigator navigator, IDictionary<string, string> xPaths, string link, Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kvp in xPaths)
            {
                results[kvp.Key] = navigator.SelectSingleNode(kvp.Value)?.Value;
            }

            onTargetRetrieved(link, results);
        }

        protected virtual void ValidateMinimalOptions()
        {
            if (mXPaths == null)
            {
                throw new InvalidOperationException($"Must first call {nameof(SetTargetPageXPaths)}.");
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
