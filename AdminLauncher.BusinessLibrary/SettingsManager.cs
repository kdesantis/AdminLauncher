using System.Drawing;

namespace AdminLauncher.BusinessLibrary
{
    public class SettingsManager : ICloneable
    {
        public OrientationsButtonEnum ButtonsOrientation { get; set; }
        public string InitialFileDialogPath { get; set; }
        public string Theme { get; set; }
        public object Clone()
        {
            return new SettingsManager()
            {
                ButtonsOrientation = ButtonsOrientation,
                InitialFileDialogPath = InitialFileDialogPath,
                Theme = Theme
            };
        }
    }
    public enum OrientationsButtonEnum
    {
        Vertical,
        Mosaic
    }
}
