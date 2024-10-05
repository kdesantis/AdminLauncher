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
        public static void LaunchInformatinError(LaunchResult launchResult)
        {
            if (launchResult.LaunchState != LaunchStateEnum.Success)
            {
                var message = $"Error in launching {launchResult.GenericItem.Name}: {launchResult.Message}";
                MessageBox.Show(message, "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static void NotifyUserForUpdate(ReleaseInformation update)
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
        public static void ErrorToSearchUpdate()
        {
            var message = $"Error in searching for updates";
            MessageBox.Show(message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
