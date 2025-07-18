﻿using AdminLauncher.UpdateLibrary;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace AdminLauncher.AppWPF.Utility
{
    public class UpdateUtility
    {
        /// <summary>
        /// Check for newer versions of the one in use
        /// </summary>
        /// <param name="showNegativeEsit"></param>
        /// <returns></returns>
        public static async Task<ReleaseInformation> CheckUpdateAsync(bool showNegativeEsit, DialogUtility dialogUtility)
        {
            var currVersion = new Version(ConfigurationManager.AppSettings["CurrVersion"]);
            var updater = new UpdateChecker(ConfigurationManager.AppSettings["UrlUpdateChecker"]);
#if DEBUG
            updater = new UpdateChecker(ConfigurationManager.AppSettings["UrlUpdateCheckerDebug"]);
#endif
            var result = await updater.CheckForUpdatesAsync(currVersion);
            if (result)
                dialogUtility.NotifyUserForUpdate(updater.UpdateInformation);
            else if (updater.Error != null)
            {
                dialogUtility.ErrorToSearchUpdate();
                return new ReleaseInformation() { Version = currVersion.ToString() };
            }
            else if (showNegativeEsit)
                dialogUtility.UpdateNotAvailable();

            return updater.UpdateInformation;
        }
        public static async void LaunchUpdateProcedure(MainWindow mainWindow, ReleaseInformation updateInformation)
        {
            if (updateInformation.Type == UrlType.Installer)
            {
                var setupPath = await new FileDownloaderUtility(mainWindow).StartDownload(updateInformation.Url, Path.Combine(Path.GetTempPath(), "setup.exe"));

                Process.Start(new ProcessStartInfo
                {
                    FileName = setupPath,
                    UseShellExecute = true
                });
                System.Windows.Application.Current.Shutdown();
            }
            else
                Process.Start(new ProcessStartInfo(updateInformation.Url) { UseShellExecute = true });
        }

    }
}
