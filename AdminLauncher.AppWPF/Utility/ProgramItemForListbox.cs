using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AdminLauncher.AppWPF.Utility
{
    public class ProgramItemForListbox
    {
        public ProgramItem Program { get; set; }
        public bool IsChecked { get; set; }

        public BitmapImage Icon => Utility.IconUtility.GetBitmapImageIcon(Program.GetIconPath());
    }
}
