using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using SHDocVw;
using System.Diagnostics;

namespace WebCrawler
{
    public static class Crawler
    {
        #region Private Fields

        private static List<WebPage> webPages = new List<WebPage>();
        private static List<string> siteDomainUrls = new List<string>();
        private static List<string> siteExternallUrls = new List<string>();
        private static List<string> siteStaticUrls = new List<string>();                
        private static Sitemap siteMap = new Sitemap();       

        #endregion

        /// <summary>
        /// Crawls a site.
        /// </summary>
        public static void CrawlSite()
        {
            Console.WriteLine("******************WebCrawler started******************");
            //Crawl the given page.
            string url = ConfigurationManager.AppSettings["url"];
            if (!string.IsNullOrEmpty(url))
            {
                CrawlPage(url);
                //add domain urls to site map.
                foreach (string link in siteDomainUrls)
                {
                    siteMap.Add(new Location()
                    {
                        Url = link,
                    });

                }
                //add external urls to site map.
                foreach (string link in siteExternallUrls)
                {
                    siteMap.Add(new Location()
                    {
                        Url = link,
                    });
                }
                //add static urls to sitemap
                foreach (string link in siteStaticUrls)
                {
                    siteMap.Add(new Location()
                    {
                        Url = link,
                    });
                }
                siteMap.SaveSiteMap(ConfigurationManager.AppSettings["pathtositemapfile"], siteMap);
                Console.WriteLine("******************WebCrawler finished*****************");
            }
            else
            {
                Console.WriteLine("URL value is empty in config file. Please provide valid URL in config file.");
            }
            
            

        }

        /// <summary>
        /// Crawls a page.
        /// </summary>
        /// <param name="url">The url to crawl.</param>
        private static void CrawlPage(string url)
        {            
            Console.WriteLine("Crawling {0} URL.",url);             
            if (!PageHasBeenCrawled(url))
                {
                    string htmlText = GetWebPageContent(url);

                    WebPage page = new WebPage();
                    page.WebPageContent = htmlText;
                    page.WebPageURL = url;
                    webPages.Add(page);

                    WebPageLinkParser linkParser = new WebPageLinkParser();
                    linkParser.ParseLinks(page, url);
                    //Add data to main data lists
                    AddRangeButNoDuplicates(siteExternallUrls, linkParser.WebPageExternalUrls);
                    AddRangeButNoDuplicates(siteStaticUrls, linkParser.WebPageStaticURLs);
                    AddRangeButNoDuplicates(siteDomainUrls, linkParser.WebPageDomainURLs);                     
                    //Again Crawl all domain links found on the page.
                    foreach (string link in linkParser.WebPageDomainURLs)
                    {
                        
                        Crawler.CrawlPage(link);
                    }
                }
                else
                {
                    return;
                }

                    
        }        

        /// <summary>
        /// Checks to see if the page has been crawled.
        /// </summary>
        /// <param name="url">The url that has potentially been crawled.</param>
        /// <returns>Boolean indicating whether or not the page has been crawled.</returns>
        private static bool PageHasBeenCrawled(string url)
        {
            foreach (WebPage page in webPages)
            {
                if (page.WebPageURL == url)
                {                    
                    return true;
                }
                    
            }           
            return false;
        }

        /// <summary>
        /// Merges a two lists of strings.
        /// </summary>
        /// <param name="targetList">The list into which to merge.</param>
        /// <param name="sourceList">The list whose values need to be merged.</param>
        private static void AddRangeButNoDuplicates(List<string> targetList, List<string> sourceList)
        {
            foreach (string str in sourceList)
            {
                if (!targetList.Contains(str))
                    targetList.Add(str);
            }
        }
        
        /// <summary>
        /// Gets the response text for a given url.
        /// </summary>
        /// <param name="url">The url whose text needs to be fetched.</param>
        /// <returns>The text of the response.</returns>
        private static string GetWebPageContent(string url)
        {

            HttpWebResponse response = null;
            string htmlText ="";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);                                
                request.UserAgent = "A .NET Web Crawler";
                request.Method = "GET";             
                response =(HttpWebResponse) request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                htmlText = reader.ReadToEnd();
            }
            catch (WebException e)
            {


                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    response = (HttpWebResponse)e.Response;
                    Console.WriteLine("Errorcode: {0}", (int)response.StatusCode);
                }
                else
                {
                    Console.WriteLine("Error: {0}", e.Status);
                }
                
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return htmlText;
        }    
    }
    public enum URLType
    {
        DomainURL,
        ExternalURL,
        StaticURL
    }
}
