# scrape-x
Simple .NET library that provides generic web scraping abilities using XPaths. Coming soon to nuget.

Basic features:
- Fluent/builder pattern interface
- Pagination
- Throttling
## Example Usage
```cs
public static void Main(string[] args)
{
	//Set up a new scraper to scrape Austin's craigslist
    Scraper scraper = new Scraper("https://austin.craigslist.org");

	//Set the URL for the results page. In this case, "apts/housing for rent".
    scraper.SetResultsStartPage("/search/apa")
    	   //Set the XPath for search result nodes
           .SetIndividualResultNodeXPath("//*[@id=\"sortable-results\"]/ul/li")           
           //Sets the XPath for search result links relative to result node
           .SetIndividualResultLinkXPath("a/@href")
           //Sets a predicate that decides whether or not an individual result should be visited or not.
           //In this case, results are only visited if their "housing" span contains "1br".
           //This saves considerable bandwidth.
           .SetResultVisitPredicate(housing => housing.Contains("1br"), "p/span[2]/span[2]")
           //Sets "Next" button link XPath
           .SetNextLink("//*[@id=\"searchform\"]/div[3]/div[3]/span[2]/a[3]/@href")
           //Sets XPaths used for retrieving data from the target page.
           //Keys are used to identify the data in the callback to the Go method.
           .SetTargetPageXPaths(new Dictionary<string, string>
           {
              { "latitude", "//*[@id=\"map\"]/@data-latitude" },
              { "longitude", "//*[@id=\"map\"]/@data-longitude" },
              { "price", "/html/body/section/section/h2/span[2]/span[1]" },
              { "br", "/html/body/section/section/section/div[1]/p[1]/span[1]/b[1]" },
              { "sqft", "/html/body/section/section/section/div[1]/p[1]/span[2]/b" }
           })
           //Go!
           //Everytime a target page is scraped this callback is called.
           .Go(OnResultRetrieved);
}
       
private static void OnResultRetrieved(string link, IDictionary<string, string> results)
{
	//Do something with the results...
    Console.WriteLine(results["br"]);
}
```
