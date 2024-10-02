using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.BusinessLibrary
{
    public static class ProcessLauncher
    {
        public static LaunchResult LaunchProgram(ProgramItem program)
        {
            var result = new LaunchResult() { LaunchState = LaunchState.Success, Message = $"program {program.Name} started correctly", GenericItem = program};
            try
            {
                if (File.Exists(program.Path))
                    System.Diagnostics.Process.Start(program.Path, program.Arguments);
                else
                    result = new LaunchResult() { LaunchState = LaunchState.Error, Message = $"{program.Path} not exist", GenericItem = program };
            }
            catch (Exception ex)
            {
                result = new LaunchResult() { LaunchState = LaunchState.Error, Message = $"Generic error: \"{ex.Message}\"", GenericItem = program };
            }
            return result;
        }
        public static void KillProgram(string processName)
        {

        }
    }
}
