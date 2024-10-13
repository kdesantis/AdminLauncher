using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class SettingsManager
    {
        //public string DataFilePath { get; set; }
        public OrientationsButtonEnum ButtonsOrientation { get; set; }

    }
    public enum OrientationsButtonEnum
    {
        Vertical,
        Horizontal
    }
}
