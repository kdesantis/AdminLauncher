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
                if (File.Exists(program.ExecutablePath))
                    switch (Path.GetExtension(program.ExecutablePath).ToLower())
                    {
                        case ".msc":
                            Process.Start("mmc.exe", program.ExecutablePath);
                            break;
                        case ".msi":
                            Process.Start("msiexec.exe", $"/i \"{program.ExecutablePath}\"");
                            break;
                        case ".ps1":
                            Process.Start("powershell.exe", $"-ExecutionPolicy Bypass -File \"{program.ExecutablePath}\"");
                            break;
                        case ".vbs":
                            Process.Start("wscript.exe", $"\"{program.ExecutablePath}\"");
                            break;
                        default:
                            Process.Start(program.ExecutablePath, program.Arguments);
                            break;
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