using System.Diagnostics;

namespace AdminLauncher.BusinessLibrary
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
