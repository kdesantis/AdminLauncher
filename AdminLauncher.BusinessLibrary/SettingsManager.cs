using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public class SettingsManager : ICloneable
    {
        public OrientationsButtonEnum ButtonsOrientation { get; set; }

        public object Clone()
        {
            return new SettingsManager() { ButtonsOrientation = ButtonsOrientation };
        }
    }
    public enum OrientationsButtonEnum
    {
        Vertical,
        Horizontal
    }
}
