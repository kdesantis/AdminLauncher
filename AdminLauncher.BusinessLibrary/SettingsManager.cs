using System.Drawing;

namespace AdminLauncher.BusinessLibrary
{
    public class SettingsManager : ICloneable
    {
        public OrientationsButtonEnum ButtonsOrientation { get; set; }
        public WindowOrientationEnum WindowOrientation { get; set; }
        public string InitialFileDialogPath { get; set; }
        public string Theme { get; set; }
        public object Clone()
        {
            return new SettingsManager()
            {
                ButtonsOrientation = ButtonsOrientation,
                WindowOrientation = WindowOrientation,
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
    public enum WindowOrientationEnum
    {
        Vertical,
        Horizontal
    }
}
