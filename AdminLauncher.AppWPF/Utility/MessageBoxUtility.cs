using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace AdminLauncher.AppWPF.Utility
{
    public static class MessageBoxUtility
    {
        /// <summary>
        /// Show the rusults of the launch occurred in case of other than positive outcome
        /// </summary>
        /// <param name="launchResult"></param>
        public static void LaunchInformatinError(LaunchResult launchResult)
        {
            if (launchResult.LaunchState != LaunchStateEnum.Success)
            {
                var message = $"Error in launching {launchResult.GenericItem.Name}: {launchResult.Message}";
                MessageBox.Show(message, "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Displays a message to the user alerting them to the presence of a newer version than the one they are using and invites them to download it
        /// </summary>
        /// <param name="update"></param>
        public static void NotifyUserForUpdate(ReleaseInformation update)
        {
            var message = $"A new version ({update.Version}) is available. Do you want to download it?\n\nRelease note:\n{update.ReleaseNotes}";
            var result = MessageBox.Show(message, "Update available", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = update.Url,
                    UseShellExecute = true
                });
            }
        }
        /// <summary>
        /// Show an error message when searching for updates
        /// </summary>
        public static void ErrorToSearchUpdate()
        {
            var message = $"Error in searching for updates";
            MessageBox.Show(message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        /// <summary>
        /// Alerts the user to the absence of available updates
        /// </summary>
        public static void UpdateNotAvailable()
        {
            var message = $"No update available. The version you have is the most recent";
            MessageBox.Show(message, "Check Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
