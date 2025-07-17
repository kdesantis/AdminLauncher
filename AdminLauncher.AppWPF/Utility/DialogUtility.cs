using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdminLauncher.AppWPF.Utility
{
    public class DialogUtility
    {
        private MainWindow MainWindow;
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
                MainWindow.ShowMessageAsync("Launch Error", message, MessageDialogStyle.Affirmative);
            }
        }
        /// <summary>
        /// Displays a message to the user alerting them to the presence of a newer version than the one they are using and invites them to download it
        /// </summary>
        /// <param name="update"></param>
        public async void NotifyUserForUpdate(ReleaseInformation update)
        {
            var message = $"A new version ({update.Version}) is available. Do you want to download it?\n\nRelease note:\n{update.ReleaseNotes}";
            var result = await MainWindow.ShowMessageAsync("Update available", message, MessageDialogStyle.AffirmativeAndNegative, GetYesNoMetroDialogSettings());
            if (result == MessageDialogResult.Affirmative)
                UpdateUtility.LaunchUpdateProcedure(MainWindow, update);
        }
        /// <summary>
        /// Show an error message when searching for updates
        /// </summary>
        public void ErrorToSearchUpdate()
        {
            var message = $"Error in searching for updates";
            MainWindow.ShowMessageAsync("Update Error", message, MessageDialogStyle.Affirmative);
        }
        /// <summary>
        /// Alerts the user to the absence of available updates
        /// </summary>
        public void UpdateNotAvailable()
        {
            var message = $"No update available. This version is the most recent";
            MainWindow.ShowMessageAsync("Check Update", message, MessageDialogStyle.Affirmative);
        }
        /// <summary>
        /// Launches a dialog box for selecting an executable. If selected it returns the path to the selected file, otherwise null
        /// </summary>
        /// <returns></returns>
        public static string ShowOpenFileDialog(string initialPath)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Supported Executable Files(*.exe,*.bat,*.cmd,*.msc,*.msi,*.ps1,*.vbs)|*.exe;*.bat;*.cmd;*.msc;*.msi;*.ps1;*.vbs|All Files (*.*)|*.*",
                AddToRecent = true
            };
            if (Directory.Exists(initialPath))
                openFileDialog.InitialDirectory = initialPath;
            if (openFileDialog.ShowDialog() == true)
                return openFileDialog.FileName;
            return null;
        }
        /// <summary>
        /// Launches a dialog box for selecting an custom icon. If selected it returns the path to the selected file, otherwise null
        /// </summary>
        /// <returns></returns>
        public static string ShowOpenFileDialogForIcon(string initialPath)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Icons(*.ico,*.png)|*.ico;*.png",
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

        public void LoadFailure(string backupPath)
        {
            var message = $"Attention, the data upload has failed! The application will be reinitialized and the saved data will be lost on the next save\n" +
                $"You will find a backup of previous file at path \"{backupPath}\"";
            MainWindow.ShowMessageAsync("Loading Failure", message, MessageDialogStyle.Affirmative);
        }
        public async Task<bool> ConfirmDeletionAsync(string itemName)
        {
            var result = await MainWindow.ShowMessageAsync("Confirm Deletion", $"Are you sure you want to delete {itemName}?", MessageDialogStyle.AffirmativeAndNegative, GetYesNoMetroDialogSettings());
            return result == MessageDialogResult.Affirmative;

        }

        public static void ShowBalloonTipAppHided(NotifyIcon notifyIcon)
        {
            notifyIcon.BalloonTipText = "Double-click the taskbar icon at the bottom right to view it. To close, right-click the icon and select “Close” from the menu.";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.BalloonTipTitle = "Admin Launcher hidden";
            notifyIcon.ShowBalloonTip(3000);
        }
        public void MultipleSessionOfApplication()
        {
            MainWindow.ShowModalMessageExternal("Non-unique session", "Another instance of this application is already running. " +
                "You can open it by double-clicking on the icon in the taskbar. " +
                "This session will be closed.", MessageDialogStyle.Affirmative);
        }

        public void ExplorerPlusPlusError()
        {
            var message = $"Error, impossible to download Explorer++. Check your Internet connection and retry";
            MainWindow.ShowMessageAsync("Impossible to downlaod Explorer++", message, MessageDialogStyle.Affirmative);
        }
        private MetroDialogSettings GetYesNoMetroDialogSettings()
        {
            var settings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
            };
            return settings;
        }
    }
}
