
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using TeamServicesApp.Model;

namespace TeamServicesApp.Clients
{
	class TeamServicesClient
	{
		private string _collectionUrl;
		private string _personalAccessToken;
		private string _projectName;

		public TeamServicesClient(string collectionUrl, string personalAccessToken, string projectName)
		{
			_collectionUrl = collectionUrl;
			_personalAccessToken = personalAccessToken;
			_projectName = projectName;
		}

		/// <summary>
		/// Downloads the work items for the query. This supports both flat
		/// queries and hierarchical. For hierarchical, this will retrieve the
		/// top two levels of the hierarchy of the query, which must be of type
		/// "Tree of work items" with a tree type of "Parent/Child".
		/// </summary>
		public TeamResult RunQuery(string folderName, string queryName)
		{
			Console.WriteLine($"Running {folderName}/{queryName}");

			// create instance of VssConnection using Personal Access Token
			var connection = new VssConnection(new Uri(_collectionUrl), new VssBasicCredential(string.Empty, _personalAccessToken));

			// get the client instance
			var witClient = connection.GetClient<WorkItemTrackingHttpClient>();

			// get 2 levels of the query hierarchy items
			var queryHierarchyItems = witClient.GetQueriesAsync(_projectName, depth: 2).Result;

			// holders for our return value
			TeamResult teamResult = null;

			// find the folder we're looking for
			var queriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals(folderName));
			if (queriesFolder != null) {

				// find the query we're looking for
				QueryHierarchyItem query = null;
				if (queriesFolder.Children != null) {
					query = queriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
				}

				var result = witClient.QueryByIdAsync(query.Id).Result;

				Console.Write($"Retrieving results ({result.QueryType} query)");

				List<TeamItem> items = null;
				switch (result.QueryType) {
					case QueryType.Tree:
						items = ProcessHierarchyItems(result, witClient);
						break;
					case QueryType.Flat:
						items = ProcessFlatItems(result, witClient);
						break;
					case QueryType.OneHop:
						throw new NotImplementedException($"{nameof(QueryType.OneHop)} not supported");
				}

				teamResult = new TeamResult
				{
					Items = items,
					QueryType = result.QueryType
				};
			}

			Console.WriteLine("done");

			return teamResult;
		}

		private List<TeamItem> ProcessHierarchyItems(WorkItemQueryResult result, WorkItemTrackingHttpClient witClient)
		{
			var hierarchyItems = new Dictionary<int, TeamItem>();
			var sortedItems = new List<TeamItem>();

			if (result.WorkItemRelations.Any()) {
				foreach (var relation in result.WorkItemRelations) {
					Console.Write(".");

					var workItem = witClient.GetWorkItemAsync(relation.Target.Id).Result;

					// create our own object to hold the item info
					var item = CreateTeamItem(workItem);

					if (relation.Source != null) {
						// it's a child, so add it to its parent
						var parentItem = hierarchyItems[relation.Source.Id];
						parentItem.Items.Add(item);
					}
					else {
						// it's a parent, so add it to our list of parents
						hierarchyItems[item.Id] = item;
						sortedItems.Add(item);
					}
				}
			}

			return sortedItems;
		}

		private List<TeamItem> ProcessFlatItems(WorkItemQueryResult result, WorkItemTrackingHttpClient witClient)
		{
			var items = new List<TeamItem>();
			foreach (var workItemRef in result.WorkItems) {
				Console.Write(".");
				var workItem = witClient.GetWorkItemAsync(workItemRef.Id).Result;
				items.Add(CreateTeamItem(workItem));
			}

			return items;
		}

		private TeamItem CreateTeamItem(WorkItem workItem)
		{
			// note that some fields may not be present in the work item so we
			// have to check for the keys
			return new TeamItem()
			{
				Id = workItem.Id.Value,
				WorkItemType = FieldStringOrNull(workItem, "System.WorkItemType"),
				Title = FieldStringOrNull(workItem, "System.Title"),
				State = FieldStringOrNull(workItem, "System.State"),
				Tags = FieldStringOrNull(workItem, "System.Tags"),
				Iteration = FieldStringOrNull(workItem, "System.IterationPath"),
				Link = ((ReferenceLink)workItem.Links.Links["html"]).Href,
				CreatedDate = (DateTime)workItem.Fields["System.CreatedDate"],
			};
		}

		private string FieldStringOrNull(WorkItem workItem, string fieldName)
		{
			if (workItem.Fields.ContainsKey(fieldName) == true) {
				return (string)workItem.Fields[fieldName];
			}
			else {
				return null;
			}
		}
	}
}
