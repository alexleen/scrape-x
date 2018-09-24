// Copyright © 2018 Alex Leendertsen

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        protected CancellationToken CancellationToken;

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

            if (CancellationToken != null)
            {
                CancellationToken.ThrowIfCancellationRequested();
            }

            ScrapeTarget(BaseUrl, onTargetRetrieved);
        }

        public Task GoAsync(Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            return Task.Run(() => Go(onTargetRetrieved));
        }

        public Task GoAsync(Action<string, IDictionary<string, string>> onTargetRetrieved, CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            return Task.Run(() => Go(onTargetRetrieved), cancellationToken);
        }

        /// <summary>
        /// Scrapes the specified link and invokes <paramref name="onTargetRetrieved"/> with the results.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="onTargetRetrieved"></param>
        protected void ScrapeTarget(string link, Action<string, IDictionary<string, string>> onTargetRetrieved)
        {
            XPathNavigator listing = Get(link);

            IDictionary<string, string> results = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kvp in mXPaths)
            {
                results[kvp.Key] = listing.SelectSingleNode(kvp.Value)?.Value;
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
    }
}
