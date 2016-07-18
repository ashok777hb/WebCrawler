using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    public class WebPageLinkParser
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebPageLinkParser() { }

        #endregion
        #region Constants

        private const string _LINK_REGEX = "href=\"[a-zA-Z./:&\\d_-]+\"";

        #endregion
        #region Private Instance Fields

        private List<string> webPageDomainURLs = new List<string>();
        private List<string> webPageBadURLs = new List<string>();
        private List<string> webPageStaticURLs = new List<string>();
        private List<string> webPageExternalUrls = new List<string>();
        private List<string> _exceptions = new List<string>();

        #endregion
        #region Public Properties

        public List<string> WebPageDomainURLs
        {
            get { return webPageDomainURLs; }
            set { webPageDomainURLs = value; }
        }

        public List<string> WebPageBadURLs
        {
            get { return webPageBadURLs; }
            set { webPageBadURLs = value; }
        }

        public List<string> WebPageStaticURLs
        {
            get { return webPageStaticURLs; }
            set { webPageStaticURLs = value; }
        }

        public List<string> WebPageExternalUrls
        {
            get { return webPageExternalUrls; }
            set { webPageExternalUrls = value; }
        }

        public List<string> Exceptions
        {
            get { return _exceptions; }
            set { _exceptions = value; }
        }

        #endregion

        /// <summary>
        /// Parses a page looking for links.
        /// </summary>
        /// <param name="page">The page whose text is to be parsed.</param>
        /// <param name="sourceUrl">The source url of the page.</param>
        public void ParseLinks(WebPage page, string sourceUrl)
        {
            MatchCollection matches = Regex.Matches(page.WebPageContent, _LINK_REGEX);

            for (int i = 0; i <= matches.Count - 1; i++)
            {
                Match anchorMatch = matches[i];

                if (anchorMatch.Value == String.Empty)
                {
                    WebPageBadURLs.Add("Blank url value on page " + sourceUrl);
                    continue;
                }

                string foundHref = null;
                try
                {
                    foundHref = anchorMatch.Value.Replace("href=\"", "");
                    foundHref = foundHref.Substring(0, foundHref.IndexOf("\""));
                }
                catch (Exception exc)
                {
                    Exceptions.Add("Error parsing matched href: " + exc.Message);
                }
                switch (FindURLType(foundHref))
                {
                    case URLType.DomainURL:
                        WebPageDomainURLs.Add(foundHref);
                        break;
                    case URLType.ExternalURL:
                        WebPageExternalUrls.Add(foundHref);
                        break;
                    case URLType.StaticURL:
                        WebPageStaticURLs.Add(foundHref);
                        break;
                    default:
                        break;
                }               
            }
        }

        public static URLType FindURLType(string url)
        {
            URLType returnType = URLType.DomainURL;

            if (url.IndexOf(ConfigurationManager.AppSettings["authority"]) > -1)
            {
                returnType= URLType.DomainURL;
            }
            else if (url.Substring(0, 7) == "http://" || url.Substring(0, 3) == "www" || url.Substring(0, 8) == "https://")
            {
                returnType= URLType.ExternalURL;
            }
            else
            {
                returnType = URLType.StaticURL;
            }
            return returnType;
        }       
    }
}
