using AdminLauncher.BusinessLibrary;
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
                .Select(e => e.ProgramItem).OrderBy(e => e.Name).ToList();
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
                        programs.Add(new InstalledProgram
                        {
                            ProgramItem = new()
                            {
                                Name = Path.GetFileNameWithoutExtension(shortcut),
                                ExecutablePath = shortcut
                            }
                        });
                    }
                }
            }
            return programs.GroupBy(e => e.ProgramItem.Name).Select(e => e.First()).ToList();
        }
    }
    public class InstalledProgram : INotifyPropertyChanged
    {
        public ProgramItem ProgramItem { get; set; }

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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
