// Copyright © 2018 Alex Leendertsen

using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using HtmlAgilityPack;
using ScrapeX.Interfaces;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace ScrapeX
{
    internal class NavigatorFactory : INavigatorFactory
    {
        public XPathNavigator Create(string url, HttpClient httpClient, HtmlWeb htmlWeb)
        {
            if (httpClient == null)
            {
                return htmlWeb.Load(url).CreateNavigator();
            }

            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response.Content.ReadAsStringAsync().Result);
            return htmlDoc.CreateNavigator();
        }
    }
}
