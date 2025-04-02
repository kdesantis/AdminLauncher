using IWshRuntimeLibrary;
using Toolbelt.Drawing;
using File = System.IO.File;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem : GenericItem, ICloneable
    {
        private string _executablePath;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string ExecutablePath
        {
            get { return _executablePath; }
            set { GetValidPath(value); }
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

            if (!string.IsNullOrEmpty(CustomIconPath))
            {
                if (File.Exists(CustomIconPath))
                    return CustomIconPath;
                else
                    logger.Error($"ProgramItem {Name}: file {CustomIconPath} not exist");
            }

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
                        logger.Error("icon extracted for {ExecutablePath} is invalid, use rocket.ico", ExecutablePath);
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

        private void GetValidPath(string path)
        {
            path = path.Replace("\"", string.Empty);
            if (Path.GetExtension(path).Equals(".lnk", StringComparison.CurrentCultureIgnoreCase))
                ResolveShortcut(path);
            else
                _executablePath = path;
        }
        private void ResolveShortcut(string shortcutPath)
        {
            var shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            var executablePath = link.TargetPath;
            var arguments = link.Arguments;

            string currentUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string usersFolderPath = Directory.GetParent(currentUserProfile).FullName;
            var UsersDirectorys = new List<string>(Directory.GetDirectories(usersFolderPath));

            var shortcutUserNameDirectory = UsersDirectorys.FirstOrDefault(e => shortcutPath.Contains(e));
            var executableUserNameDirectory = UsersDirectorys.FirstOrDefault(e => link.TargetPath.Contains(e));


            // Correct TargetPath with correct user
            if (!Path.Exists(executablePath) &&
                !string.IsNullOrEmpty(shortcutUserNameDirectory) &&
                !string.IsNullOrEmpty(executableUserNameDirectory) &&
                shortcutUserNameDirectory != executableUserNameDirectory)
            {
                logger.Debug($"{executablePath} replaced with {executablePath.Replace(executableUserNameDirectory, shortcutUserNameDirectory)}");
                executablePath = executablePath.Replace(executableUserNameDirectory, shortcutUserNameDirectory);
            }

            _executablePath = executablePath;
            Arguments = arguments;
        }

        public object Clone()
        {
            return new ProgramItem
            {
                _executablePath = this._executablePath,
                Arguments = this.Arguments,
                IsFavorite = this.IsFavorite,
                Index = this.Index,
                Name = this.Name,
                CustomIconPath = this.CustomIconPath
            };
        }
        
        public bool Equals(ProgramItem programItem2)
        {
            return programItem2.Name == this.Name
                && programItem2.Index == this.Index
                && programItem2.ExecutablePath == this.ExecutablePath 
                && programItem2.Arguments == this.Arguments;
        }
    }
}
