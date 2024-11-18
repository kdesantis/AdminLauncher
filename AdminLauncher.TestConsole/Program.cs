using System;
using System.Collections.Generic;
using System.IO;
using AdminLauncher.BusinessLibrary;
using IWshRuntimeLibrary;
class Program
{
    static void Main()
    {
        var installedPrograms = GetProgramsFromStartMenu();

        foreach (var program in installedPrograms)
        {
            Console.WriteLine($"Nome: {program.Name}");
            Console.WriteLine($"Percorso: {program.ExecutablePath}");
            Console.WriteLine($"Argomenti: {program.Arguments}");
            Console.WriteLine();
        }
    }

    static List<ProgramItem> GetProgramsFromStartMenu()
    {
        List<ProgramItem> programs = new List<ProgramItem>();

        // Percorsi del menu Start
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

    static ProgramItem GetShortcutDetails(string shortcutPath)
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

