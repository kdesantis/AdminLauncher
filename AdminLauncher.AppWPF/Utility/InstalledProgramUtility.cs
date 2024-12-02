﻿using AdminLauncher.BusinessLibrary;
using IWshRuntimeLibrary;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace AdminLauncher.AppWPF.Utility
{
    public class InstalledProgramUtility
    {
        private NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private List<string> EXECUTABLEEXTENSION = new() { ".exe", ".cmd", ".bat", ".vbs", ".msc", ".msi", ".ps1" };
        private List<string> UsersDirectorys;

        public InstalledProgramUtility()
        {
            string currentUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string usersFolderPath = Directory.GetParent(currentUserProfile).FullName;
            UsersDirectorys = new List<string>(Directory.GetDirectories(usersFolderPath));
        }

        public List<ProgramItem> GetInstalledProgram()
        {
            return GetProgramsFromStartMenu()
                .Select(e => new ProgramItem() { Name = e.Name, Arguments = e.Arguments, ExecutablePath = e.ExecutablePath }).OrderBy(e => e.Name).ToList();
        }

        private List<InstalledProgram> GetProgramsFromStartMenu()
        {
            List<InstalledProgram> programs = new List<InstalledProgram>();

            List<string> startMenuPaths = new()
            {
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
            };
            foreach (string userDir in UsersDirectorys)
            {
                try
                {
                    string startMenuPath = Path.Combine(userDir, @"AppData\Roaming\Microsoft\Windows\Start Menu");
                    if (Directory.Exists(startMenuPath))
                        startMenuPaths.Add(startMenuPath);
                }
                catch (Exception ex)
                {
                    logger.Warn(ex, $"Error to access  in the directory {userDir}");
                }
            }

            foreach (var startMenuPath in startMenuPaths)
            {
                string programsPath = Path.Combine(startMenuPath, "Programs");

                if (Directory.Exists(programsPath))
                {
                    var shortcuts = Directory.GetFiles(programsPath, "*.lnk", SearchOption.AllDirectories);

                    foreach (var shortcut in shortcuts)
                    {
                        var shortcutDetails = GetShortcutDetails(shortcut);
                        if (!string.IsNullOrEmpty(shortcutDetails.ExecutablePath)
                            && System.IO.File.Exists(shortcutDetails.ExecutablePath)
                            && EXECUTABLEEXTENSION.Contains(Path.GetExtension(shortcutDetails.ExecutablePath).ToLower()))
                            programs.Add(shortcutDetails);
                    }
                }
            }
            return programs.GroupBy(e => e.Name).Select(e => e.First()).ToList();
        }

        private InstalledProgram GetShortcutDetails(string shortcutPath)
        {
            try
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                var executablePath = shortcut.TargetPath;
                var arguments = shortcut.Arguments;

                var shortcutUserNameDirectory = UsersDirectorys.FirstOrDefault(e => shortcutPath.Contains(e));
                var executableUserNameDirectory = UsersDirectorys.FirstOrDefault(e => shortcut.TargetPath.Contains(e));

                // Correct TargetPath with correct user
                if (!Path.Exists(executablePath) &&
                    !string.IsNullOrEmpty(shortcutUserNameDirectory) &&
                    !string.IsNullOrEmpty(executableUserNameDirectory) &&
                    shortcutUserNameDirectory != executableUserNameDirectory)
                {
                    logger.Debug($"{executablePath} replaced with {executablePath.Replace(executableUserNameDirectory, shortcutUserNameDirectory)}");
                    executablePath = executablePath.Replace(executableUserNameDirectory, shortcutUserNameDirectory);
                }

                return new InstalledProgram
                {
                    Name = Path.GetFileNameWithoutExtension(shortcutPath),
                    ExecutablePath = executablePath,
                    Arguments = arguments
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, "shortcutPath:{shortcutPath}", shortcutPath);
                return new InstalledProgram
                {
                    Name = Path.GetFileNameWithoutExtension(shortcutPath),
                    ExecutablePath = null,
                    Arguments = null
                };
            }
        }
    }
    public class InstalledProgram : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public BitmapSource GetIcon()
        {
            if (System.IO.File.Exists(ExecutablePath))
            {
                Icon icon = Icon.ExtractAssociatedIcon(ExecutablePath);
                return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }

            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
