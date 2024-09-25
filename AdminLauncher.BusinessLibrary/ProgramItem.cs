using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AdminLauncher.BusinessLibrary
{
    public class ProgramItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Arguments { get; set; }
        public bool IsFavorite { get; set; }

        public void Launch()
        {
            ProcessLauncher.LaunchProgram(this);
        }

    }
}
