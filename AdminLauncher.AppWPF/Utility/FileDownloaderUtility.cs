using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AdminLauncher.AppWPF.Utility
{
    internal class FileDownloaderUtility
    {
        private double _downloadProgress;
        private MainWindow MainWindow;
        private CancellationTokenSource _cancellationTokenSource;
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public FileDownloaderUtility(MainWindow MainWindow)
        {
            this.MainWindow = MainWindow;
        }
        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                _downloadProgress = value;
                OnPropertyChanged();
            }
        }
        private async Task DownloadFileAsync(string url, string destinationPath, IProgress<double> progress, CancellationToken cancellationToken)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                   fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var totalBytesRead = 0L;
                var buffer = new byte[8192];
                var isMoreToRead = true;

                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        progress.Report(100);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                    totalBytesRead += bytesRead;
                    if (totalBytes != -1)
                    {
                        progress.Report((double)totalBytesRead / totalBytes * 100);
                    }
                }
                while (isMoreToRead);
            }
        }


        public async Task<string> StartDownload(string url, string destinationPath)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                var controller = await MainWindow.ShowProgressAsync("Download in progress", "Downloading the file, please wait...", true);
                controller.SetIndeterminate();

                controller.Canceled += async (sender, args) =>
                {
                    _cancellationTokenSource.Cancel();
                    await controller.CloseAsync();
                };

                var progress = new Progress<double>(value => controller.SetProgress(value / 100));
                await DownloadFileAsync(url, destinationPath, progress, cancellationToken);

                await controller.CloseAsync();
            }
            catch (OperationCanceledException)
            {
                logger.Warn($"Download of {url} cancelled by User");
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error during download of {url}");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            return destinationPath;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
