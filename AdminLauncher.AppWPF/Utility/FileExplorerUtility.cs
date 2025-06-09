using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Manager _manager;
        private DialogUtility _dialogUtility;
        private MainWindow _mainWindows;

        public FileExplorerUtility(Manager manager, DialogUtility dialogUtility, MainWindow mainWindow)
        {
            _manager = manager;
            _dialogUtility = dialogUtility;
            _mainWindows = mainWindow;
        }

        public async void LaunchFileExplorer()
        {
            if (FileExplorerExist())
            {
                var releaseInfo = await GitHubReleaseFetcher.GetLatestReleaseAsync("derceg", "explorerplusplus");
                if (releaseInfo is not null && releaseInfo.TagName != _manager.settingsManager.ExplorerPlusPlusTagName)
                    await DownlaoadLastVersion(releaseInfo);
                StartFileExplorer();
            }
            else
            {
                var releaseInfo = await GitHubReleaseFetcher.GetLatestReleaseAsync("derceg", "explorerplusplus");
                if (releaseInfo != null)
                {
                    await DownlaoadLastVersion(releaseInfo);
                    LaunchFileExplorer();
                }
                else
                {
                    _dialogUtility.ExplorerPlusPlusError();
                }
            }
        }

        private async Task DownlaoadLastVersion(GitHubReleaseInfo releaseInfo)
        {
            Kill();
            var url = releaseInfo.Assets.FirstOrDefault(a => a.DownloadUrl.Contains("explorerpp_x64.zip")).DownloadUrl;
            var destinationpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus.zip");
            var downloader = new FileDownloaderUtility(_mainWindows);
            await downloader.StartDownload(url, destinationpath);
            if (destinationpath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus");
                Directory.CreateDirectory(extractPath);
                ExtractZip(destinationpath, extractPath);
            }
            File.Delete(destinationpath);
            _manager.settingsManager.ExplorerPlusPlusTagName = releaseInfo.TagName;
            _manager.Save();
        }

        private bool FileExplorerExist()
        {
            return Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus"));
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
        private void StartFileExplorer()
        {
            if (Process.GetProcessesByName("Explorer++").Count() == 0)
            {
                var result = (new ProgramItem { Name = "File Explorer", ExecutablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "explorerplusplus", "Explorer++.exe") }).Launch();
                _dialogUtility.LaunchInformatinError(result);
            }
        }
        public void Kill()
        {
            Process.GetProcessesByName("Explorer++").FirstOrDefault()?.Kill();
        }
    }
}
