using AdminLauncher.BusinessLibrary;
using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminLauncher.AppWPF.Utility
{
    public class InstalledProgramUtility
    {

        public static List<ProgramItem> GetInstalledProgram()
        {
            return GetProgramsFromStartMenu();
        }

        private static List<ProgramItem> GetProgramsFromStartMenu()
        {
            List<ProgramItem> programs = new List<ProgramItem>();

            string[] startMenuPaths = new string[]
            {
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
            };

            foreach (var startMenuPath in startMenuPaths)
            {
                string programsPath = Path.Combine(startMenuPath, "Programs");

                if (Directory.Exists(programsPath))
                {
                    var shortcuts = Directory.GetFiles(programsPath, "*.lnk", SearchOption.AllDirectories);

                    foreach (var shortcut in shortcuts)
                    {
                        var shortcutDetails = GetShortcutDetails(shortcut);

                        if (!string.IsNullOrEmpty(shortcutDetails.ExecutablePath) && System.IO.File.Exists(shortcutDetails.ExecutablePath))
                        {
                            programs.Add(shortcutDetails);
                        }
                    }
                }
            }
            return programs;
        }

        private static ProgramItem GetShortcutDetails(string shortcutPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                return new ProgramItem
                {
                    Name = Path.GetFileNameWithoutExtension(shortcutPath),
                    ExecutablePath = shortcut.TargetPath,
                    Arguments = shortcut.Arguments
                };
            }
            catch
            {
                return new ProgramItem
                {
                    Name = Path.GetFileNameWithoutExtension(shortcutPath),
                    ExecutablePath = null,
                    Arguments = null
                };
            }
        }
    }
}
