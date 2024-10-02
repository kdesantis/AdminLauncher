using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
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
            if (launchResult.LaunchState != LaunchState.Success)
            {
                var message = $"Error in launching {launchResult.GenericItem.Name}: {launchResult.Message}";
                MessageBox.Show(message, "Launch Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
