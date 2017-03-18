
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
			var folderName = "My Queries";
			var queryName = "New Query";

			// use the arguments from the command line if we got any
			if(args.Length == 2) {
				folderName = args[0];
				queryName = args[1];
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
				Console.WriteLine("You must set the Team Services Personal Access Token, Colection URL, and project name in the app.config");
				return;
			}

			// create our team services client
			var client = new TeamServicesClient(url, token, project);

			// retrieve the result set for the given hierarchical query
			// (e.g. a query that returns epics with features as children)
			var items = client.RunHierarchyQuery(folderName, queryName);

			// shorten the urls in the items
			BitlyClient.ShortenUrlsForItems(items);

			// export the query results as a csv file
			CsvExporter.ExportHierarchy(queryName, items);
		}
	}
}
