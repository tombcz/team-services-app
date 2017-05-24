
using System;
using System.IO;
using TeamServicesApp.Model;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace TeamServicesApp.Exporters
{
	class CsvExporter
	{
		/// <summary>
		/// Creates a CSV file on the user's desktop (overwriting any existing file).
		/// This will simply list each work item if the items are flat, or it will
		/// list all the children of the items along with the title of the parent if
		/// the items are hierarchical.
		/// </summary>
		public static void ExportItems(string queryName, TeamResult result)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			path = Path.Combine(path, $"{queryName}.csv");

			Console.Write($"Exporting to {path}");

			using (var file = new StreamWriter(path)) {
				if (result.QueryType == QueryType.Tree) {

					// these are hierarchical, so list each child and include the
					// title of its parent
					foreach (var parent in result.Items) {
						foreach (var child in parent.Items) {
							Console.Write(".");
							EmitItem(file, parent, child);
						}
					}
				}
				else {

					// these are flat, so just list each item
					foreach (var item in result.Items) {
						Console.Write(".");
						EmitItem(file, null, item);
					}
				}
			}

			Console.WriteLine("done");
		}

		private static void EmitItem(StreamWriter file, TeamItem parent, TeamItem child)
		{
			var id = child.Id;
			var title = FormatValue(child.Title);
			var state = FormatValue(child.State);
			var createdDate = child.CreatedDate.ToShortDateString();
			var tags = FormatValue(child.Tags);
			var iteration = FormatValue(child.Iteration);
			var link = FormatValue(child.Link);

			if (parent != null) {
				var parentTitle = FormatValue(parent.Title);
				file.WriteLine($"{id},{parentTitle},{title},{state},{createdDate},{tags},{iteration},{link}");
			}
			else {
				file.WriteLine($"{id},{title},{state},{createdDate},{tags},{iteration},{link}");
			}
		}

		private static string FormatValue(string s)
		{
			if (s == null) {
				return string.Empty;
			}
			return s.Replace(",", "");
		}
	}
}
