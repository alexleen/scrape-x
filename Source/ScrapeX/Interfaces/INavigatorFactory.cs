﻿// Copyright © 2018 Alex Leendertsen

using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using HtmlAgilityPack;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ScrapeX.Interfaces
{
    internal interface INavigatorFactory
    {
        /// <summary>
        /// Creates an <see cref="XPathNavigator"/> by retrieving the HTML from the specified <paramref name="url"/>
        /// using either the specified <see cref="HttpClient"/> (if not null) or the specified <see cref="HtmlWeb"/>.
        /// If <paramref name="httpClient"/> is null, <paramref name="htmlWeb"/> must not be.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        XPathNavigator Create(string url, HttpClient httpClient, HtmlWeb htmlWeb);
    }
}
