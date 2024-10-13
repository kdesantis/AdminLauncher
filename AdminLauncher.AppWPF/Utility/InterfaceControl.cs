using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using System.Configuration;
using System.Windows;

namespace AdminLauncher.AppWPF.Utility
{
    public static class InterfaceControl
    {
        /// <summary>
        /// Set the position of MainWindows to the bottom right, above the program bar
        /// </summary>
        /// <param name="mainWindow"></param>
        public static void PositionWindowInBottomRight(MainWindow mainWindow)
        {
            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaWidth = SystemParameters.WorkArea.Width;

            mainWindow.Left = workAreaWidth - mainWindow.Width;
            mainWindow.Top = workAreaHeight - mainWindow.Height;
        }
        private static void ClearAddProgramData(MainWindow mainWindow)
        {
            mainWindow.ProgramIndexLabel.Content = -1;
            mainWindow.ProgramNameTextBox.Clear();
            mainWindow.ProgramPathTextBox.Clear();
            mainWindow.ProgramArgumentsTextBox.Clear();
            mainWindow.FavoriteCheckBox.IsChecked = false;
        }
        private static void ClearRoutineData(MainWindow mainWindow)
        {
            mainWindow.RoutineIndexLabel.Content = -1;
            mainWindow.RoutineNameTextBox.Clear();
            mainWindow.ProgramsListBox.UnselectAll();
        }
        /// <summary>
        /// Makes the desired StackPanel visible
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="mainWindow"></param>
        public static void InterfaceLoader(InterfaceEnum mode, MainWindow mainWindow)
        {
            mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
            mainWindow.SettingsPanel.Visibility = Visibility.Collapsed;
            mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
            mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
            mainWindow.AbountPanel.Visibility = Visibility.Collapsed;

            switch (mode)
            {
                case InterfaceEnum.Home:
                    mainWindow.MainScrollViewer.Visibility = Visibility.Visible;
                    ClearAddProgramData(mainWindow);
                    ClearRoutineData(mainWindow);
                    break;
                case InterfaceEnum.Settings:
                    mainWindow.SettingsPanel.Visibility = Visibility.Visible;
                    break;
                case InterfaceEnum.AddProgramInterface:
                    mainWindow.AddProgramPanel.Visibility = Visibility.Visible;
                    break;
                case InterfaceEnum.AddRoutineInterface:
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Visible;
                    break;
                case InterfaceEnum.About:
                    mainWindow.AbountPanel.Visibility = Visibility.Visible;
                    break;
            }
        }
        /// <summary>
        /// Populates the Listbox with programs that you can select to create/edit a routine
        /// </summary>
        /// <param name="programs"></param>
        /// <param name="mainWindow"></param>
        /// <param name="routineToUpdate"></param>
        public static void LoadProgramsListBox(List<ProgramItem> programs, MainWindow mainWindow, RoutineItem routineToUpdate = null)
        {
            mainWindow.ProgramsListBox.ItemsSource = programs;
            if (routineToUpdate != null)
                programs.Where(e => routineToUpdate.Programs.Select(e => e.Index).Contains(e.Index)).ToList()
                    .ForEach(program => mainWindow.ProgramsListBox.SelectedItems.Add(program));
        }
        /// <summary>
        /// Manages the display of items regarding version and updates in the AboutTab
        /// </summary>
        /// <param name="updateInfo"></param>
        /// <param name="mainWindow"></param>
        public static void UpdateVersionText(ReleaseInformation updateInfo, MainWindow mainWindow)
        {
            var currVersion = new Version(ConfigurationManager.AppSettings["CurrVersion"]);
            mainWindow.CurrentVersionText.Text = currVersion.ToString();
            mainWindow.LastVersionText.Text = updateInfo.Version.ToString();
            if (new Version(updateInfo.Version) > currVersion)
            {
                mainWindow.UpdateLink.Visibility = Visibility.Visible;
                mainWindow.UpdateLinkHyperLink.NavigateUri = new Uri(updateInfo.Url);
            }
            else
            {
                mainWindow.UpdateLink.Visibility = Visibility.Collapsed;
                mainWindow.CheckUpdateLink.Visibility = Visibility.Visible;
            }
        }

        public static void LoadButtonsOrienationComboBox(MainWindow mainWindow, Manager manager)
        {
            mainWindow.ButtonsOrientationCombobox.ItemsSource = Enum.GetValues(typeof(OrientationsButtonEnum));
            mainWindow.ButtonsOrientationCombobox.SelectedItem = manager.settingsManager.ButtonsOrientation;
        }
    }
    /// <summary>
    /// Indentifies the various tabs in the ui
    /// </summary>
    public enum InterfaceEnum
    {
        Home,
        Settings,
        AddProgramInterface,
        AddRoutineInterface,
        About
    }
}
