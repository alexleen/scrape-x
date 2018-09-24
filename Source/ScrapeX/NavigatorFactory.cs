// Copyright © 2018 Alex Leendertsen

using HtmlAgilityPack;
using ScrapeX.Interfaces;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        //TODO Is there any benefit to this? We can't do anything while waiting...
        public async Task<XPathNavigator> CreateAsync(string url, HttpClient httpClient, HtmlWeb htmlWeb)
        {
            HtmlDocument htmlDoc;

            if (httpClient == null)
            {
                htmlDoc = await htmlWeb.LoadFromWebAsync(url);
            }
            else
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();

                htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);
            }

            return htmlDoc.CreateNavigator();
        }
    }
}
