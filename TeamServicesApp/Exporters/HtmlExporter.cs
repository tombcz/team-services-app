
using System;
using System.IO;
using TeamServicesApp.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace TeamServicesApp.Exporters
{
	class HtmlExporter
	{
		/// <summary>
		/// Creates an HTML file on the user's desktop (overwriting any existing file).
		/// This will simply list each work item if the items are flat, or it will
		/// list all the children of the items along with the title of the parent if
		/// the items are hierarchical.
		/// </summary>
		public static void ExportItems(string queryName, TeamResult result)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			path = Path.Combine(path, $"{queryName}.html");

			Console.Write($"Exporting to {path}");

			using (var file = new StreamWriter(path)) {

				file.WriteLine(@"<html>
    <head>
    <style>
        body {
            font-family: ""-apple-system"", BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Helvetica, Ubuntu, Arial, sans-serif, ""Apple Color Emoji"", ""Segoe UI Emoji"", ""Segoe UI Symbol"";
			}


        .idnumber {
            font-weight: 700;
            font-size: 12px;
            color: #000000;
            text-decoration: underline;
        }

        .badge {
            display: inline-block;
            border-radius: 3px;
            padding-bottom: 2px;
            padding-top: 2px;
            padding-left: 5px;
            padding-right: 5px;
            font-size: 8px;
            font-weight: 600
        }

        .tag {
            background: #78909C;
            color: #ffffff;
        }

        .state {
            background: #673AB7;
            color: #ffffff;
        }

        .iteration {
            background: #2196F3;
            color: #ffffff;
        }

.title{
    line-height: 25px;
    font-size: 14px;
}

.parentTitle{
    line-height: 45px;
    font-size: 18px;
}

    </style></head>");

				if (result.QueryType == QueryType.Tree) {

					var previousParentTitle = string.Empty;

					// these are hierarchical, so list each child and include the
					// title of its parent
					foreach (var parent in result.Items) {
						foreach (var child in parent.Items) {
							Console.Write(".");

							if (previousParentTitle != parent.Title) {
								EmitParentItem(file, parent);
								previousParentTitle = parent.Title;
							}

							EmitItem(file, child);
						}
					}
				}
				else {

					// these are flat, so just list each item
					foreach (var item in result.Items) {
						Console.Write(".");
						EmitItem(file, item);
					}
				}

				file.WriteLine("</body></html>");
			}

			Console.WriteLine("done");
		}

		private static void EmitParentItem(StreamWriter file, TeamItem child)
		{
			var id = child.Id;
			var title = FormatValue(child.Title);
			var state = FormatValue(child.State);
			var createdDate = child.CreatedDate.ToShortDateString();
			var tags = FormatValue(child.Tags);
			var iteration = FormatValue(child.Iteration);
			var link = FormatValue(child.Link);

			file.WriteLine($"<span class=\"parentTitle\">{title}</span>");
			file.WriteLine("<br>");
		}

		private static void EmitItem(StreamWriter file, TeamItem child)
		{
			var id = child.Id;
			var title = FormatValue(child.Title);
			var state = FormatValue(child.State);
			var createdDate = child.CreatedDate.ToShortDateString();
			var tags = FormatValue(child.Tags);
			var iteration = FormatValue(child.Iteration);
			var link = FormatValue(child.Link);

			file.WriteLine($"<a href=\"{link}\"><span class=\"idnumber\">{id}</span></a>");

			file.WriteLine($"<span class=\"title\">{title}</span>");

			//file.WriteLine($"<span class=\"date\">{createdDate}</span>");

			if (tags != null) {
				var tagList = tags.Split(',');
				foreach (var tag in tagList) {
					file.WriteLine($"<span class=\"badge tag\">{tag.ToUpper()}</span>");
				}
			}

			file.WriteLine($"<span class=\"badge state\">{state.ToUpper()}</span>");
			file.WriteLine($"<span class=\"badge iteration\">{iteration.ToUpper()}</span>");

			file.WriteLine("<br>");
		}

		private static string FormatValue(string s)
		{
			if (s == null) {
				return string.Empty;
			}
			return s.Replace("&", "&amp;");
		}
	}
}
