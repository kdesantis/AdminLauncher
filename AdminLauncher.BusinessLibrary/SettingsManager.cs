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
