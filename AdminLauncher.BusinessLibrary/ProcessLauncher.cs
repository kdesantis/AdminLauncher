﻿namespace AdminLauncher.BusinessLibrary
{
    public static class ProcessLauncher
    {
        /// <summary>
        /// Given the path to an executable, launch the application as an independent process
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static LaunchResult LaunchProgram(ProgramItem program)
        {
            var result = new LaunchResult() { LaunchState = LaunchStateEnum.Success, Message = $"program {program.Name} started correctly", GenericItem = program };
            try
            {
                if (File.Exists(program.ExecutablePath))
                    System.Diagnostics.Process.Start(program.ExecutablePath, program.Arguments);
                else
                    result = new LaunchResult() { LaunchState = LaunchStateEnum.Error, Message = $"{program.ExecutablePath} not exist", GenericItem = program };
            }
            catch (Exception ex)
            {
                result = new LaunchResult() { LaunchState = LaunchStateEnum.Error, Message = $"Generic error: \"{ex.Message}\"", GenericItem = program };
            }
            return result;
        }
    }
}
