using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using System.Diagnostics;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdminLauncher.AppWPF.Utility
{
    public static class DialogUtility
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
            var message = $"No update available. This version is the most recent";
            MessageBox.Show(message, "Check Update", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        /// <summary>
        /// Launches a dialog box for selecting an executable. If selected it returns the path to the selected file, otherwise null
        /// </summary>
        /// <returns></returns>
        public static string ShowOpenFileDialog()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Executable Files (*.exe)(*.bat)(*.cmd)|*.exe;*.bat;*.cmd|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            return null;
        }

        public static void LoadFailure()
        {
            var message = $"Attention, the data upload has failed! The application will be reinitialized and the saved data will be lost on the next save";
            MessageBox.Show(message, "Loading Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static bool ConfirmDeletion(string itemName)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {itemName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            return result == MessageBoxResult.Yes;
        }
    }
}
