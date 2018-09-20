using HtmlAgilityPack;
using ScrapeX.Interfaces;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Xml.XPath;

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
