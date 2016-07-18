# WebCrawler
Description:-
It is a simple web crawler written in C#.

It will do following tasks:

	->It will crawl all domain URL's.
	->It won't  do crawling of external URL's.
	->It will write all domain, external and static URL's to sitemap.xml file.
	->It will show the URL name in console output while crawling URL.

How to Use this web crawler:-
  
	->This project is created using Visual studio 2013 premium edition.
	  so you need VS2013 to compile this project.
	->Download the project from https://github.com/ashok777hb/WebCrawler/archive/master.zip
	->Then change the following parameters in app.config file according to your choice.
	    
		<add key="url" value="http:\\wiprodigital.com"/> <!-- What site do you want to crawl? -->
      
		<add key="authority" value="wiprodigital.com"/> <!-- The authority is the server dns hostname or ip address. -->
      
		<add key="pathtositemapfile" value=".\sitemap.xml"/><!--Path to file sitemap.xml file.-->
  
	->Compile and run executable from bin folder. The output file sitemap.xml will be generated on the configured folder.
