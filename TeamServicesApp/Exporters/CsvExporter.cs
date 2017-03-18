
using System;
using System.Collections.Generic;
using System.IO;
using TeamServicesApp.Model;

namespace TeamServicesApp.Exporters
{
	class CsvExporter
	{
		/// <summary>
		/// Creates a CSV file on the user's desktop (overwriting any existing file) that
		/// lists all the children of the top-level items provided.
		/// </summary>
		public static void ExportHierarchy(string queryName, List<TeamItem> items)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			path = Path.Combine(path, $"{queryName}.csv");

			Console.Write($"Exporting to {path}");

			var links = new List<string>();
			using (var file = new StreamWriter(path)) {
				// pass the parent item into the emitter so that we can get a nice
				// denormalized csv output
				foreach (var parent in items) {
					foreach (var child in parent.Items) {
						Console.Write(".");
						EmitCsvItem(file, parent, child, links);
					}
				}
			}

			Console.WriteLine("done");
		}

		private static void EmitCsvItem(StreamWriter file, TeamItem parent, TeamItem child, List<string> links)
		{
			var id = child.Id;
			var parentTitle = FormatValue(parent.Title);
			var title = FormatValue(child.Title);
			var state = FormatValue(child.State);
			var createdDate = child.CreatedDate.ToShortDateString();
			var tags = FormatValue(child.Tags);
			var iteration = FormatValue(child.Iteration);
			var link = FormatValue(child.Link);
			file.WriteLine($"{id},{parentTitle},{title},{state},{createdDate},{tags},{iteration},{link}");
		}

		private static string FormatValue(string s)
		{
			if(s == null) {
				return string.Empty;
			}
			return s.Replace(",", "");
		}
	}
}
