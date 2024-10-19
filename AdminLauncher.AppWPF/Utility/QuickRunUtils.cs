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
        public static void LaunchQuickRun()
        {
            var filePath = DialogUtility.ShowOpenFileDialog();
            if (filePath is not null)
            {
                var result = (new ProgramItem { Name = "Quick Run", ExecutablePath = filePath }).Launch();
                DialogUtility.LaunchInformatinError(result);
            }
        }
    }
}
