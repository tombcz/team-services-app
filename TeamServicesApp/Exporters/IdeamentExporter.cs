
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TeamServicesApp.Model;

namespace TeamServicesApp.Exporters
{
	/// <summary>
	/// Creates a document for the Ideament app (iOS and Windows 10).
	/// </summary>
	class IdeamentExporter
	{
		private static void Write(string queryName, List<TeamItem> workItems)
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			path = Path.Combine(path, $"{queryName}.ideament");

			var links = new List<string>();
			using (StreamWriter file = new StreamWriter(path)) {

				var root_item_guid = Guid.NewGuid().ToString();

				file.WriteLine("<sketch version=\"103\">");
				file.WriteLine("<project version=\"5\">");
				file.WriteLine("<lineStyle>0</lineStyle>");
				file.WriteLine("<arrowStyle>0</arrowStyle>");
				file.WriteLine("<fontFamily>0</fontFamily>");
				file.WriteLine("<colorTheme id=\"0\">");
				file.WriteLine("<theme id=\"0\" rgb=\"#EF9A9A\" />"); // red
				file.WriteLine("<theme id=\"1\" rgb=\"#A5D6A7\" />"); // green
				file.WriteLine("<theme id=\"2\" rgb=\"#81D4FA\" />"); // light blue
				file.WriteLine("<theme id=\"3\" rgb=\"#90CAF9\" />"); // blue
				file.WriteLine("<theme id=\"4\" rgb=\"#FFCC80\" />"); // orange
				file.WriteLine("<theme id=\"5\" rgb=\"#B0BEC5\" />"); // blue gray
				file.WriteLine("<theme id=\"6\" rgb=\"#B39DDB\" />"); // purple
				file.WriteLine("<theme id=\"7\" rgb=\"#FFFFFF\" />"); // white
				file.WriteLine("</colorTheme>");
				file.WriteLine("<shapes>");

				file.WriteLine($"<shape color=\"0\" expanded=\"1\" guid=\"{ root_item_guid }\" kind=\"0\" order=\"0\" fontSize=\"0\" size=\"0\">");
				file.WriteLine("<name>9AE187D8-BD76-4099-9472-2057E21202A4</name>");
				file.WriteLine("<center x=\"1621.654053\" y=\"1588.444092\" />");
				file.WriteLine("</shape>");

				foreach (var item in workItems) {
					var itemGuid = Guid.NewGuid().ToString();
					EmitItem(itemGuid, file, item, root_item_guid, links);
					foreach (var child in item.Items) {
						EmitItem(Guid.NewGuid().ToString(), file, child, itemGuid, links);
					}
				}

				file.WriteLine("</shapes>");
				file.WriteLine("<links>");
				foreach (var next_link_line in links) {
					file.WriteLine(next_link_line);
				}
				file.WriteLine("</links>");

				file.WriteLine("</project>");
				file.WriteLine("</sketch>");
			}
		}

		private static void EmitItem(string itemGuid, StreamWriter file, TeamItem item, string parentItemGuid, List<string> links)
		{
			int color;
			if (item.State == "Closed") {
				color = 1;
			}
			else {
				color = 0;
			}
			
			var itemTitle = item.Title;
			var title = $"{itemTitle}";
			var subtitle = $"#{item.Id} {item.Link}";
			file.WriteLine($"<shape color=\"{ color }\" expanded=\"1\" guid=\"{ itemGuid }\" kind=\"0\" order=\"0\" fontSize=\"0\" size=\"0\">");
			file.WriteLine($"<name>{ XmlEscape(title.Trim()) }</name>");
			file.WriteLine($"<notes>{ XmlEscape(subtitle.Trim()) }</notes>");
			file.WriteLine("<center x=\"1750\" y=\"1750\" />");
			file.WriteLine("</shape>");
			links.Add($"<link guid=\"{ Guid.NewGuid() }\" childShape=\"{ itemGuid }\" parentShape=\"{ parentItemGuid }\" />");
		}

		public static string XmlEscape(string unescaped)
		{
			var doc = new XmlDocument();
			var node = doc.CreateElement("root");
			node.InnerText = unescaped;
			return node.InnerXml;
		}
	}
}
