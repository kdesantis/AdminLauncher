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
    public class UpdateUtility
    {
        public static async Task CheckUpdateAsync(Version currVersion)
        {
            var updater = new UpdateChecker();
            if (await updater.CheckForUpdatesAsync(currVersion))
                NotifyUserForUpdate(updater.UpdateInformation);
        }

        private static void NotifyUserForUpdate(ReleaseInformation update)
        {
            // Mostra una finestra di dialogo per notificare l'utente
            var message = $"A new version ({update.Version}) is available. Do you want to download it?\n\nRelease note:\n{update.ReleaseNotes}";
            var result = MessageBox.Show(message, "Update available", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                // Apri il browser con l'URL per scaricare l'aggiornamento
                Process.Start(new ProcessStartInfo
                {
                    FileName = update.Url,
                    UseShellExecute = true
                });
            }
        }
    }
}
