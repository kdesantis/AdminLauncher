using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbelt.Drawing;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem : GenericItem
    {
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value.Replace("\"",string.Empty); }
        }

        public string? Arguments { get; set; }
        public bool IsFavorite { get; set; }

        public override void Launch()
        {
            ProcessLauncher.LaunchProgram(this);
        }

        public override string GetIconPath()
        {
            var iconPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{Index}-{Name}.ico");
            if (File.Exists(Path))
            {
                var s = File.Create(iconPath);
                IconExtractor.Extract1stIconTo(Path, s);
                bool valid = s.Length > 0;
                s.Close();
                return valid ? iconPath : null;
            }
            return null;
        }

    }
}
