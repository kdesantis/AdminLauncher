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
        public async void LaunchFileExplorer(Manager manager, DialogUtility dialogUtility)
        {

            if (FileExplorerExist())
            {
                var releaseInfo = await GitHubReleaseFetcher.GetLatestReleaseAsync("derceg", "explorerplusplus");
                if (releaseInfo is not null && releaseInfo.TagName != manager.settingsManager.ExplorerPlusPlusTagName)
                    await DownlaoadLastVersion(manager, dialogUtility, releaseInfo);
                StartFileExplorer(dialogUtility);
            }
            else
            {
                var releaseInfo = await GitHubReleaseFetcher.GetLatestReleaseAsync("derceg", "explorerplusplus");
                if (releaseInfo != null)
                {
                    await DownlaoadLastVersion(manager, dialogUtility, releaseInfo);
                }
                else
                {
                    dialogUtility.ExplorerPlusPlusError();
                }
            }

        }

        private async Task DownlaoadLastVersion(Manager manager, DialogUtility dialogUtility, GitHubReleaseInfo releaseInfo)
        {
            var stateDownload = await DownloadFileExplorerAsync(dialogUtility, releaseInfo);
            if (stateDownload)
            {
                manager.settingsManager.ExplorerPlusPlusTagName = releaseInfo.TagName;
                manager.Save();
            }
        }

        private bool FileExplorerExist()
        {
            return Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus"));
        }

        private async Task<bool> DownloadFileExplorerAsync(DialogUtility dialogUtility, GitHubReleaseInfo releaseInfo)
        {
            if (releaseInfo is not null)
            {
                var url = releaseInfo.Assets.FirstOrDefault(a => a.DownloadUrl.Contains("explorerpp_x64.zip")).DownloadUrl;
                var downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus.zip");
                await DownloadFileAsync(url, downloadPath);

                if (downloadPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus");
                    Directory.CreateDirectory(extractPath);
                    ExtractZip(downloadPath, extractPath);
                }
                File.Delete(downloadPath);
                return true;
            }
            dialogUtility.ExplorerPlusPlusError();
            return false;
        }
        public static async Task DownloadFileAsync(string url, string outputPath)
        {
            HttpClient _client = new HttpClient();
            // GitHub richiede uno User-Agent
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpApp");

            using var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }
        public static void ExtractZip(string zipPath, string destinationDirectory)
        {
            if (File.Exists(zipPath) && zipPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                ZipFile.ExtractToDirectory(zipPath, destinationDirectory, overwriteFiles: true);
            }
        }
        private void StartFileExplorer(DialogUtility dialogUtility)
        {
            var result = (new ProgramItem { Name = "File Explorer", ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus", "Explorer++.exe") }).Launch();
            dialogUtility.LaunchInformatinError(result);

        }
    }
}
