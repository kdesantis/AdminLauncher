using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminLauncher.AppWPF.Utility
{
    public static class GitHubReleaseFetcher
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<GitHubReleaseInfo> GetLatestReleaseAsync(string owner, string repo)
        {
            var url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp");

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var releaseJson = JsonDocument.Parse(json).RootElement;

            var release = new GitHubReleaseInfo
            {
                TagName = releaseJson.GetProperty("tag_name").GetString(),
                Name = releaseJson.GetProperty("name").GetString(),
                HtmlUrl = releaseJson.GetProperty("html_url").GetString(),
                Assets = new List<GitHubAsset>()
            };

            foreach (var asset in releaseJson.GetProperty("assets").EnumerateArray())
            {
                release.Assets.Add(new GitHubAsset
                {
                    Name = asset.GetProperty("name").GetString(),
                    DownloadUrl = asset.GetProperty("browser_download_url").GetString()
                });
            }

            return release;
        }
    }
    public class GitHubReleaseInfo
    {
        public string TagName { get; set; }
        public string Name { get; set; }
        public string HtmlUrl { get; set; }
        public List<GitHubAsset> Assets { get; set; }
    }

    public class GitHubAsset
    {
        public string Name { get; set; }
        public string DownloadUrl { get; set; }
    }

}
