using IWshRuntimeLibrary;
using Toolbelt.Drawing;
using File = System.IO.File;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem : GenericItem
    {
        private string executablePath;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
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
            logger.Info($"start GetIconPath;{Index};{Name}");
            var directoryPath = Path.Combine(Path.GetTempPath(), "AdminLauncherTempIcon");
            var iconPath = Path.Combine(directoryPath, $"{Index}-{Path.GetFileName(ExecutablePath)}.ico");

            //Generic icon for script
            if (new List<string>() { ".vbs", ".cmd", ".bat", ".ps1" }.Contains(Path.GetExtension(ExecutablePath).ToLower()))
            {
                logger.Info("Executable file don't have icon; use PowerShellIcon.png icon");
                iconPath = Path.Combine(directoryPath, $"99999-genericPromptIcon.png");
                if (!File.Exists(iconPath))
                    File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PowerShellIcon.png"), iconPath, true);
            }

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            //Add this to avoid crashes in the event of an inability to recover
            try
            {
                if (File.Exists(ExecutablePath))
                {
                    if (File.Exists(iconPath) && new FileInfo(iconPath).Length > 0)
                    {
                        logger.Info("exit - icon already exists");
                        return iconPath;
                    }
                    logger.Info("start extract icon from exe");
                    using var s = File.Create(iconPath);
                    IconExtractor.Extract1stIconTo(ExecutablePath, s);
                    bool valid = s.Length > 0;
                    if (!valid)
                    {
                        logger.Error("icon extracted is invalid, use rocket.ico");
                        s.Close();
                        File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"), iconPath, true);
                    }
                    return iconPath;
                }
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "path:{iconPath}", iconPath);
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"), iconPath, true);
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
