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
        /// <summary>
        /// Check for newer versions of the one in use
        /// </summary>
        /// <param name="showNegativeEsit"></param>
        /// <returns></returns>
        public static async Task<ReleaseInformation> CheckUpdateAsync(bool showNegativeEsit)
        {
            var currVersion = new Version(ConfigurationManager.AppSettings["CurrVersion"]);
            var updater = new UpdateChecker(ConfigurationManager.AppSettings["UrlUpdateChecker"]);
            var result = await updater.CheckForUpdatesAsync(currVersion);
            if (result)
                MessageBoxUtility.NotifyUserForUpdate(updater.UpdateInformation);
            else if (updater.Error != null)
                MessageBoxUtility.ErrorToSearchUpdate();
            else if (showNegativeEsit)
                MessageBoxUtility.UpdateNotAvailable();

            return updater.UpdateInformation;
        }

    }
}
