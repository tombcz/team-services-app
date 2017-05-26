
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TeamServicesApp.Model;

namespace TeamServicesApp.Clients
{
	public class BitlyClient
	{
		/// <summary>
		/// You can get a free access token from https://dev.bitly.com
		/// </summary>
		public static string AccessToken { get; set; }

        public static void ShortenUrls(IList<TeamItem> items)
        {
            Console.Write("Shortening URLs");
            ShortenUrlsRecursive(items);
            Console.WriteLine("done");
        }

        private static void ShortenUrlsRecursive(IList<TeamItem> items)
		{
			if (string.IsNullOrEmpty(AccessToken) == false) {
				foreach (var item in items) {
					item.Link = ShortenUrl(item.Link).Result;
					foreach (var child in item.Items) {
						Console.Write(".");
						item.Link = ShortenUrl(item.Link).Result;
					}
                    ShortenUrlsRecursive(item.Items);
				}
			}
		}

		private static async Task<string> ShortenUrl(string url)
		{
			var encodedUrl = WebUtility.UrlEncode(url);
			var callUrl = $"https://api-ssl.bitly.com/v3/shorten?access_token={AccessToken}&longUrl={encodedUrl}&format=txt";
			using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) }) {
				HttpResponseMessage response = await client.GetAsync(callUrl).ConfigureAwait(false);
				return await response.Content.ReadAsStringAsync();
			}
		}
	}
}
