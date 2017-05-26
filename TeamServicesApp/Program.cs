
using TeamServicesApp.Exporters;
using System.Configuration;
using TeamServicesApp.Clients;
using System;

// Required NuGet Packages:
// Microsoft.TeamFoundationServer.Client
// Microsoft.TeamFoundationServer.ExtendedClient

namespace TeamServicesApp
{
	class Program
	{
		static void Main(string[] args)
		{
			// set our default folder and query names
			string folderName = null;
            string queryName = null;
            string exportFormat = null;

			// use the arguments from the command line if we got any
			if (args.Length == 3) {
				folderName = args[0];
				queryName = args[1];
                exportFormat = args[2];
			}
            else
            {
                throw new ArgumentException("Command line arguments not provided");
            }

			// give our bitly client our access token if there is one (not required)
			BitlyClient.AccessToken = ConfigurationManager.AppSettings["BitlyAccessToken"];

			// grab our team services info
			var token = ConfigurationManager.AppSettings["TeamServicesPersonalAccessToken"];
			var url = ConfigurationManager.AppSettings["TeamServicesCollectionUrl"];
			var project = ConfigurationManager.AppSettings["TeamServicesProjectName"];

			// make sure we have the necessary info
			if(string.IsNullOrEmpty(token) == true ||
				string.IsNullOrEmpty(url) == true ||
				string.IsNullOrEmpty(project) == true) {
				Console.WriteLine("You must set the Team Services Personal Access Token, Colection URL, and project name in the App.config");
				return;
			}

			// create our team services client
			var client = new TeamServicesClient(url, token, project);

			// retrieve the result set for the given query
			var result = client.RunQuery(folderName, queryName);

			// shorten the urls in the items
			BitlyClient.ShortenUrls(result.Items);

            // export the query results
            switch (exportFormat)
            {
                case "csv":
                    CsvExporter.ExportItems(queryName, result);
                    break;
                case "html":
                    HtmlExporter.ExportItems(queryName, result);
                    break;           
            }
		}
	}
}
