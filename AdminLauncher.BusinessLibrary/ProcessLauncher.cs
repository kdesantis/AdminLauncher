using System;
using System.Diagnostics;

namespace AdminLauncher.BusinessLibrary
{
    public static class ProcessLauncher
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Given the path to an executable, launch the application as an independent process
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static LaunchResult LaunchProgram(ProgramItem program)
        {
            logger.Info($"start LaunchProgram {program.Name};{program.ExecutablePath};{program.Arguments}");
            var result = new LaunchResult() { LaunchState = LaunchStateEnum.Success, Message = $"program {program.Name} started correctly", GenericItem = program };
            try
            {
                Process process = new Process();
                if (File.Exists(program.ExecutablePath))
                {
                    switch (Path.GetExtension(program.ExecutablePath).ToLower())
                    {
                        case ".msc":
                            process = Process.Start("mmc.exe", program.ExecutablePath);
                            break;
                        case ".msi":
                            process = Process.Start("msiexec.exe", $"/i \"{program.ExecutablePath}\"");
                            break;
                        case ".ps1":
                            process = Process.Start("powershell.exe", $"-ExecutionPolicy Bypass -File \"{program.ExecutablePath}\"");
                            break;
                        case ".vbs":
                            process = Process.Start("wscript.exe", $"\"{program.ExecutablePath}\"");
                            break;
                        default:
                            process = Process.Start(program.ExecutablePath, program.Arguments);
                            break;
                    }
                    if (process.ExitCode != 0)
                    {
                        logger.Error($"Exit code {process.ExitCode}. Program {program.ExecutablePath} - {program.Arguments}");
                        result = new LaunchResult() { LaunchState = LaunchStateEnum.Error, Message = $"{process.ExitCode}", GenericItem = program };
                    }
                }
                else
                {
                    logger.Info($"executable file not exists{program.ExecutablePath}");
                    result = new LaunchResult() { LaunchState = LaunchStateEnum.Error, Message = $"{program.ExecutablePath} not exist", GenericItem = program };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = new LaunchResult() { LaunchState = LaunchStateEnum.Error, Message = $"Generic error: \"{ex.Message}\"", GenericItem = program };
            }
            return result;
        }
    }
}
