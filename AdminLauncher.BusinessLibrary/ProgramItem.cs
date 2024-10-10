using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
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
            var iconPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Index}-{Name}.ico");
            if (File.Exists(ExecutablePath))
            {
                var s = File.Create(iconPath);
                IconExtractor.Extract1stIconTo(ExecutablePath, s);
                bool valid = s.Length > 0;
                s.Close();
                return valid ? iconPath : null;
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
