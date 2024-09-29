using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbelt.Drawing;

namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem: GenericItem
    {
        public string Path { get; set; }
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
                s.Close();
                return iconPath;
            }
            return null;
        }

    }
}
