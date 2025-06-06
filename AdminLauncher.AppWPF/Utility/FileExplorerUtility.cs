using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.AppWPF.Utility
{
    internal class FileExplorerUtility
    {

        public void LaunchFileExplorer()
        {
            if(FileExplorerExist())
                StartFileExplorer();
            else
            {
                DownloadFileExplorerAsync();
                LaunchFileExplorer();
            }
        }

        private bool FileExplorerExist()
        {
            return Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus"));
        }

        private async Task DownloadFileExplorerAsync()
        {

            var release = await GitHubReleaseFetcher.GetLatestReleaseAsync("derceg", "explorerplusplus");
            var url = release.Assets.FirstOrDefault(a => a.DownloadUrl.Contains("explorerpp_x64.zip")).DownloadUrl;
            var downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus.zip");
            await GitHubDownloader.DownloadFileAsync(url, downloadPath);

            if (downloadPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus");
                Directory.CreateDirectory(extractPath);
                ZipExtractor.ExtractZip(downloadPath, extractPath);
            }
        }

        private void StartFileExplorer()
        {
            var result = (new ProgramItem { Name = "File Explorer", ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus", "Explorer++.exe") }).Launch();
            //dialogUtility.LaunchInformatinError(result);
            
        }

    }
    public static class GitHubDownloader
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task DownloadFileAsync(string url, string outputPath)
        {
            // GitHub richiede uno User-Agent
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp");

            using var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }
    }
    public static class ZipExtractor
    {
        public static void ExtractZip(string zipPath, string destinationDirectory)
        {
            if (File.Exists(zipPath) && zipPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"Estraendo {Path.GetFileName(zipPath)}...");
                ZipFile.ExtractToDirectory(zipPath, destinationDirectory, overwriteFiles: true);
                Console.WriteLine($"Estratto in: {destinationDirectory}");
            }
            else
            {
                Console.WriteLine($"Il file {zipPath} non è uno ZIP o non esiste.");
            }
        }
    }
}
