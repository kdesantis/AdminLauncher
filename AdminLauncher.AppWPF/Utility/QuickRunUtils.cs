using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminLauncher.AppWPF.Utility
{
    public static class QuickRunUtils
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void LaunchQuickRun(string initialPath, DialogUtility dialogUtility)
        {
            logger.Info("Start QuickRun");
            var filePath = DialogUtility.ShowOpenFileDialog(initialPath);
            if (filePath is not null)
            {
                var result = (new ProgramItem { Name = "Quick Run", ExecutablePath = filePath }).Launch();
                dialogUtility.LaunchInformatinError(result);
            }
        }
    }
}
