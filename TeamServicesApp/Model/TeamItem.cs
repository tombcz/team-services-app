
using System;
using System.Collections.Generic;
using System.Text;

namespace TeamServicesApp.Model
{
	public class TeamItem
	{
		public int Id { get; set; }
		public string WorkItemType { get; set;  }
		public string Title { get; set;  }
		public string State { get; set; }
		public string Link { get; set; }
		public string Tags { get; set; }
		public DateTime CreatedDate { get; set;  }
		public string Iteration { get; set; }

		public List<TeamItem> Items { get; set; }

		public TeamItem()
		{
			Items = new List<TeamItem>();
		}

		public override string ToString()
		{
			var s = new StringBuilder();
			s.Append($"Id:{Id}{Environment.NewLine}");
			s.Append($"Title:{Title}{Environment.NewLine}");
			s.Append($"Type:{WorkItemType}{Environment.NewLine}");
			s.Append($"State:{State}{Environment.NewLine}");
			s.Append($"Link:{Link}{Environment.NewLine}");
			return s.ToString();
		}
	}
}
