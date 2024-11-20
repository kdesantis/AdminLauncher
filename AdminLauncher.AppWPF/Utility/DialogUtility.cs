using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdminLauncher.AppWPF.Utility
{
    public class DialogUtility
    {
        private MainWindow MainWindow = new();
        public DialogUtility(MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
        }
        /// <summary>
        /// Show the rusults of the launch occurred in case of other than positive outcome
        /// </summary>
        /// <param name="launchResult"></param>
        public void LaunchInformatinError(LaunchResult launchResult)
        {
            if (launchResult.LaunchState != LaunchStateEnum.Success)
            {
                var message = $"Error in launching {launchResult.GenericItem.Name}: {launchResult.Message}";
                //MessageBox.Show(message, "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
                 MainWindow.ShowMessageAsync("Launch Error", message);
            }
        }
        /// <summary>
        /// Displays a message to the user alerting them to the presence of a newer version than the one they are using and invites them to download it
        /// </summary>
        /// <param name="update"></param>
        public async void NotifyUserForUpdate(ReleaseInformation update)
        {
            var message = $"A new version ({update.Version}) is available. Do you want to download it?\n\nRelease note:\n{update.ReleaseNotes}";
            //var result = MessageBox.Show(message, "Update available", MessageBoxButton.YesNo, MessageBoxImage.Information);
            var result = await MainWindow.ShowMessageAsync("Update available", message, MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
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
        public void ErrorToSearchUpdate()
        {
            var message = $"Error in searching for updates";
            //MessageBox.Show(message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            MainWindow.ShowMessageAsync("Update Error", message);
        }
        /// <summary>
        /// Alerts the user to the absence of available updates
        /// </summary>
        public void UpdateNotAvailable()
        {
            var message = $"No update available. This version is the most recent";
            //MessageBox.Show(message, "Check Update", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow.ShowMessageAsync("Check Update", message, MessageDialogStyle.Affirmative, GetMetroDialogSettingsError());
        }
        /// <summary>
        /// Launches a dialog box for selecting an executable. If selected it returns the path to the selected file, otherwise null
        /// </summary>
        /// <returns></returns>
        public static string ShowOpenFileDialog(string initialPath)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Supported Executable Files|*.exe;*.bat;*.cmd;*.msc;*.msi;*.ps1;*.vbs|All Files (*.*)|*.*",
                AddToRecent = true
            };
            if (Directory.Exists(initialPath))
                openFileDialog.InitialDirectory = initialPath;
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            return null;
        }

        public static string ShowOpenFolderDialog()
        {
            OpenFolderDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FolderName;
            return null;
        }

        public void LoadFailure()
        {
            var message = $"Attention, the data upload has failed! The application will be reinitialized and the saved data will be lost on the next save";
            MessageBox.Show(message, "Loading Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
            MainWindow.ShowMessageAsync("Loading Failure", message, MessageDialogStyle.Affirmative);
        }
        public async Task<bool> ConfirmDeletionAsync(string itemName)
        {
            //MessageBoxResult result = MessageBox.Show(
            //    $"Are you sure you want to delete {itemName}?",
            //    "Confirm Deletion",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Warning
            //);
            var result = await MainWindow.ShowMessageAsync("Confirm Deletion", $"Are you sure you want to delete {itemName}?", MessageDialogStyle.AffirmativeAndNegative);
            return result == MessageDialogResult.Affirmative;
            //return result == MessageBoxResult.Yes;

        }

        public static void ShowBalloonTipAppHided(NotifyIcon notifyIcon)
        {
            notifyIcon.BalloonTipText = "You can view it by double-clicking on the icon in the taskbar at the bottom right of the screen.\r\n" +
                "If you want to close it, right-click on the icon and select the “Close” option from the drop-down menu.";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = "Admin Launcher hided";
            notifyIcon.ShowBalloonTip(3000);
        }
        public void MultipleSessionOfApplication()
        {
            //MessageBox.Show("Another instance of this application is already running. " +
            //    "You can open it by double-clicking on the icon in the taskbar. " +
            //    "This session will be closed.", "Non-unique session", MessageBoxButton.OK, MessageBoxImage.Warning);

            MainWindow.ShowMessageAsync("Non-unique session", "Another instance of this application is already running. " +
                "You can open it by double-clicking on the icon in the taskbar. " +
                "This session will be closed.");
        }

        private MetroDialogSettings GetMetroDialogSettingsError()
        {
            var settings = new MetroDialogSettings()
            {
                AnimateShow = true,
            };
            return settings;
        }
    }
}
