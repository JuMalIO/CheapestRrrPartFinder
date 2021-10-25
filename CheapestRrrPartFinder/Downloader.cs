using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace CheapestRrrPartFinder
{
    public class Downloader
    {
        public List<(decimal price, string url)> Download(string url, Action<int, int> callback)
        {
            var list = new List<(decimal price, string url)>();
            var html = DownloadHtml(url);
            var totalPages = ParseTotalPages(html);

            for (var i = 1; i <= totalPages; i++)
            {
                if (i != 1)
                {
                    html = DownloadHtml(url + $"&page={i}");
                }

                list.AddRange(ParsePrieceAndUrl(html));

                callback(i, totalPages);
            }

            return list;
        }

        private static string DownloadHtml(string url)
        {
            using (var client = new WebClient())
            {
                var htmlCode = client.DownloadString(url);
                return htmlCode;
            }
        }

        private static int ParseTotalPages(string html)
        {
            const string searchFor = "span class=\"pages__links\">";

            var i = html.IndexOf(searchFor);
            var str = html.Substring(i + searchFor.Length, html.Length - i - searchFor.Length);
            i = str.IndexOf("<");
            var values = str.Substring(0, i).Split("/");


            return int.Parse(values[1].Trim());
        }

        private static List<(decimal price, string url)> ParsePrieceAndUrl(string html)
        {
            int index;
            var list = new List<(decimal price, string url)>();

            const string searchFor = "<h3><a href=\"https://rrr.lt/autodalis";
            const string searchFor2 = "Kaina: <strong>";

            while ((index = html.IndexOf(searchFor)) > 0)
            {
                html = html.Substring(index + searchFor.Length, html.Length - index - searchFor.Length);
                index = html.IndexOf("\"");

                string url = $"https://rrr.lt/autodalis{(html.Substring(0, index))}";

                index = html.IndexOf(searchFor2);
                html = html.Substring(index + searchFor2.Length, html.Length - index - searchFor2.Length);
                index = html.IndexOf(" ");

                decimal price = decimal.Parse(html.Substring(0, index), CultureInfo.InvariantCulture);

                list.Add((price, url));
            }
            return list;
        }
    }
}
