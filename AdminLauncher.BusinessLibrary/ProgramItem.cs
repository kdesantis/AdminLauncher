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

        [JsonPropertyName("Path")]
        public string ExecutablePath
        {
            get { return executablePath; }
            set { executablePath = GetValidPath(value); }
        }

        public string? Arguments { get; set; }
        public bool IsFavorite { get; set; }

        public override LaunchResult Launch()
        {
            return ProcessLauncher.LaunchProgram(this);
        }

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
            if (Path.GetExtension(path).ToLower() == ".lnk")
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
