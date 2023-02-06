using BlazingPicdump.Extensions;
using BlazingPicdump.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazingPicdump
{
    public static partial class Helper
    {

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }

        public static string SHA256ToString(string s)
        {
            return string.Join(null, SHA256.HashData(Encoding.UTF8.GetBytes(s)).Select(x => x.ToString("x2")));
        }

        public static async Task<List<Picdump>> GetPicdumpOverview(string input, HttpClient httpClient, CancellationToken cancellationToken)
        {
            Regex links_regex = GetPicdumpOverView();
            List<Picdump> picdumps = new();
            var matches = links_regex.Matches(input);

            await Parallel.ForEachAsync(matches, async (match, cancellationToken) =>
            {
                try
                {
                    if (match.Success)
                    {
                        var url = match.Groups["url"].Value;
                        var title = match.Groups["title"].Value;
                        var description = match.Groups["desc"].Value;
                        if (!picdumps.Any(x => x.Name == title))
                        {
                            var thumbailUrl = match.Groups["tumbnail"].Value;
                            var base64Payload = await DownloadImageAsBase64($"/http://www.bildschirmarbeiter.com/{thumbailUrl}", httpClient, cancellationToken);
                            var picdump = new Picdump
                            {
                                Name = title,
                                BaseURL = url,
                                Thumbnail = new Image() { Name = "thumbnail", Url = $"data:image/png;base64,{base64Payload}" },
                                Description = description,
                                Hash = SHA256ToString(title)
                            };
                            picdumps.Add(picdump);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("{ErroMessage}",ex.Message);
                }

            });

            return picdumps;
        }

        public async static Task<DateTime> GetPicdumpDate(HttpClient httpClient, string baseUrl)
        {

            var response = await httpClient.GetAsync($"/{baseUrl}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Regex dateTitle_regex = GetPicumpDate();
            var match = dateTitle_regex.Match(content);
            if (match.Success)
            {
                return DateTime.Parse(match.Groups["date"].Value);
            }
            else
            {
                throw new Exception("Failed to extract date");
            }
        }

        public static async Task<string> DownloadImageAsBase64(string imgUrl, HttpClient client, CancellationToken cancellationToken)
        {
            var response = await client.GetAsync(imgUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return stream.ConvertToBase64();
        }

        public static List<string> GetPicdumpImageUrls(string input)
        {
            Regex images_regex = ExtractImages();
            var matches = images_regex.Matches(input);

            return matches.Select(match =>
            {
                return match.Groups["src"].Value;
            }).ToList();
        }

        public static async Task<List<Image>> GetAllPicdumpImages(string baseUrl, HttpClient httpClient, CancellationToken cancellationToken)
        {
            List<Image> images = new();
            Regex images_regex = ExtractImages();

            var response = await httpClient.GetAsync($"/{baseUrl}", cancellationToken);
            response.EnsureSuccessStatusCode();
            string input = await response.Content.ReadAsStringAsync(cancellationToken);

            var matches = images_regex.Matches(input);

            await Parallel.ForEachAsync(matches, async (match, cancellationToken) =>
            {
                if (match.Success)
                {
                    var imageUrl = match.Groups["src"].Value;
                    Console.WriteLine(imageUrl);
                    var imageFileName = Path.GetFileName(imageUrl);
                    var base64Payload = await DownloadImageAsBase64($"/{imageUrl}", httpClient, cancellationToken);
                    images.Add(new Image() { Name = imageFileName, Url = $"data:image/png;base64,{base64Payload}" });
                }
            });

            return images;

        }

        [GeneratedRegex("<img\\sclass=.image.\\sid=.pic[0-9]{1,3}.\\ssrc=.(?<src>http:\\/{2}.*\\.(jpg|jpeg|png))")]
        private static partial Regex ExtractImages();
        [GeneratedRegex("<div class=.contenthead.>\n\\s*<span>(?<date>[0-9]{2}.[0-9]{2}.[0-9]{4})<\\/span>\n\\s*<h1>(?<title>.*)<\\/h1>")]
        private static partial Regex GetPicumpDate();
        [GeneratedRegex("<img class=.plugthumb.\\ssrc=.(?<tumbnail>\\/content\\/plugs\\/thumbs2\\/cache\\/picdump_kw[0-9]{2}_[0-9]{4}.*\\.jpg).*\\n\\s*<h2><a class=.title.\\shref\\s*=\\s*.(?<url>[a-zA-Z]{3,4}:\\/{2}w{3}\\.bildschirmarbeiter\\.com\\/pic\\/(nachschlag_-_bildschirmarbeiter|bildschirmarbeiter)_-_picdump_kw_*[0-9]{2}_[0-9]{4})\\/.>(?<title>.*)<\\/a><\\/h2>\\n\\s*<div class=.desc.>\\n\\s*<p>(?<desc>.*)")]
        private static partial Regex GetPicdumpOverView();
    }
}
