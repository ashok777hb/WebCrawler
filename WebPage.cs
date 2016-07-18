using System;

namespace WebCrawler
{
    public class WebPage
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebPage() { }

        #endregion
        #region Private Instance Fields
        
        private string _webPageContent;
        private string _webPageURL;

        #endregion
        #region Public Properties

 
        public string WebPageContent
        {
            get { return _webPageContent; }
            set
            {
                _webPageContent = value;
               
            }
        }

        public string WebPageURL
        {
            get { return _webPageURL; }
            set { _webPageURL = value; }
        }      
        #endregion       
    }
}
