using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public static class ProcessLauncher
    {
        public static void LaunchProgram(ProgramItem program)
        {
            var proc = System.Diagnostics.Process.Start(program.Path, program.Arguments);
        }
        public static void KillProgram(string processName)
        {

        }
    }
}
