namespace AdminLauncher.UpdateLibrary
{
    public class ReleaseInformation
    {
        public string Version { get; set; }
        public UrlType Type { get; set; }
        public string Url { get; set; }
        public string ReleaseNotes { get; set; }
    }
    public enum UrlType
    {
        Link = 0,
        Installer = 1
    }
}
