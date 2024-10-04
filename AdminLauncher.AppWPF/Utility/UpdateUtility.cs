using AdminLauncher.UpdateLibrary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace AdminLauncher.AppWPF.Utility
{
    public class UpdateUtility
    {
        public static async Task<ReleaseInformation> CheckUpdateAsync(Version currVersion)
        {
            var updater = new UpdateChecker(ConfigurationManager.AppSettings["UrlUpdateChecker"]);
            var result = await updater.CheckForUpdatesAsync(currVersion);
            if (result)
                NotifyUserForUpdate(updater.UpdateInformation);
            else if (updater.Error != null)
            {
                var message = $"Error in searching for updates";
                MessageBox.Show(message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return updater.UpdateInformation;
        }
        private static void NotifyUserForUpdate(ReleaseInformation update)
        {
            var message = $"A new version ({update.Version}) is available. Do you want to download it?\n\nRelease note:\n{update.ReleaseNotes}";
            var result = MessageBox.Show(message, "Error in searching for updates", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = update.Url,
                    UseShellExecute = true
                });
            }
        }
    }
}
