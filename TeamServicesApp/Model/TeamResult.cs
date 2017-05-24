
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;

namespace TeamServicesApp.Model
{
	class TeamResult
	{
		public List<TeamItem> Items { get; set; }
		public QueryType QueryType { get; set; }
	}
}
