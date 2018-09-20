using HtmlAgilityPack;
using System.Net.Http;
using System.Xml.XPath;

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
