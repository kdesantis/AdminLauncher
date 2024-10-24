namespace AdminLauncher.BusinessLibrary
{
    public class SettingsManager : ICloneable
    {
        public OrientationsButtonEnum ButtonsOrientation { get; set; }
        public string InitialFileDialogPath { get; set; }
        public object Clone()
        {
            return new SettingsManager()
            {
                ButtonsOrientation = ButtonsOrientation,
                InitialFileDialogPath = InitialFileDialogPath
            };
        }
    }
    public enum OrientationsButtonEnum
    {
        Vertical,
        Mosaic
    }
}
