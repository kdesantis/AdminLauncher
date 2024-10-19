using IWshRuntimeLibrary;
using Toolbelt.Drawing;
using File = System.IO.File;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem : GenericItem
    {
        private string executablePath;
        public string ExecutablePath
        {
            get { return executablePath; }
            set { executablePath = GetValidPath(value); }
        }

        public string? Arguments { get; set; }
        public bool IsFavorite { get; set; }
        /// <summary>
        /// Launches the executable of the current program
        /// </summary>
        /// <returns></returns>
        public override LaunchResult Launch()
        {
            return ProcessLauncher.LaunchProgram(this);
        }
        /// <summary>
        /// Extracts the exe icon to a temporary folder and returns the path to access it
        /// </summary>
        /// <returns></returns>
        public override string GetIconPath()
        {
            var directoryPath = Path.Combine(Path.GetTempPath(), "AdminLauncherTempIcon");
            var iconPath = Path.Combine(directoryPath, $"{Index}-{Path.GetFileName(ExecutablePath)}.ico");

            if (!Directory.Exists(iconPath))
                Directory.CreateDirectory(directoryPath);

            if (File.Exists(ExecutablePath))
            {
                if (File.Exists(iconPath) && new FileInfo(iconPath).Length > 0)
                    return iconPath;
                using var s = File.Create(iconPath);
                IconExtractor.Extract1stIconTo(ExecutablePath, s);
                bool valid = s.Length > 0;
                if (!valid)
                {
                    s.Close();
                    File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"), iconPath, true);
                }
                return iconPath;
            }
            return null;
        }

        private static string GetValidPath(string path)
        {
            path = path.Replace("\"", string.Empty);
            if (Path.GetExtension(path).Equals(".lnk", StringComparison.CurrentCultureIgnoreCase))
                path = ResolveShortcut(path);
            return path;
        }
        private static string ResolveShortcut(string shortcutPath)
        {
            var shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            return link.TargetPath;
        }
    }
}
