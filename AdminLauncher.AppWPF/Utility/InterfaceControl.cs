using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using ControlzEx.Theming;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

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

            if (workAreaWidth < mainWindow.Width)
                mainWindow.Width = workAreaWidth;
            if (workAreaHeight < mainWindow.Height)
                mainWindow.Height = workAreaHeight;
          
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
            mainWindow.ProgramIconPathTextBox.Clear();
        }
        private static void ClearRoutineData(MainWindow mainWindow)
        {
            mainWindow.RoutineIndexLabel.Content = -1;
            mainWindow.RoutineNameTextBox.Clear();
            mainWindow.RoutineIconPathTextBox.Clear();

            if (mainWindow.ProgramsListBox.ItemsSource is not null)
            {
                var items = mainWindow.ProgramsListBox.ItemsSource as List<ProgramItemForListbox>;
                if (items != null)
                {
                    foreach (var item in items)
                        item.IsChecked = false;
                    mainWindow.ProgramsListBox.Items.Refresh();
                }
            }
        }
        /// <summary>
        /// Makes the desired StackPanel visible
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="mainWindow"></param>
        public static void InterfaceLoader(InterfaceEnum mode, MainWindow mainWindow)
        {

            switch (mode)
            {
                case InterfaceEnum.Home:
                    mainWindow.MainTabControl.SelectedItem = mainWindow.HomeTab;
                    ClearAddProgramData(mainWindow);
                    ClearRoutineData(mainWindow);
                    break;
                case InterfaceEnum.Settings:
                    mainWindow.MainTabControl.SelectedItem = mainWindow.SettingsTab;
                    break;
                case InterfaceEnum.AddProgramInterface:
                    mainWindow.MainTabControl.SelectedItem = mainWindow.AddProgramTab;
                    break;
                case InterfaceEnum.AddRoutineInterface:
                    mainWindow.MainTabControl.SelectedItem = mainWindow.AddRoutineTab;
                    break;
                case InterfaceEnum.About:
                    mainWindow.MainTabControl.SelectedItem = mainWindow.AboutTab;
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
            var ProgramForListbox = programs.Select(e => new ProgramItemForListbox() { Program = e, IsChecked = false }).OrderBy(e => e.Program.Name).ToList();
            if (routineToUpdate != null)
            {
                ProgramForListbox.Where(e => routineToUpdate.Programs.Select(e => e.Index).Contains(e.Program.Index)).ToList()
                    .ForEach(program => ProgramForListbox.First(e => e.Program.Index == program.Program.Index).IsChecked = true);
            }
            mainWindow.ProgramsListBox.ItemsSource = ProgramForListbox;
        }
        /// <summary>
        /// Manages the display of items regarding version and updates in the AboutTab
        /// </summary>
        /// <param name="updateInfo"></param>
        /// <param name="mainWindow"></param>
        public static void UpdateVersionText(ReleaseInformation updateInfo, MainWindow mainWindow)
        {
            mainWindow.UpdateLink.Visibility = Visibility.Collapsed;
            mainWindow.CheckUpdateLink.Visibility = Visibility.Collapsed;
            var currVersion = new Version(ConfigurationManager.AppSettings["CurrVersion"]);
            mainWindow.CurrentVersionText.Text = currVersion.ToString();
            mainWindow.LastVersionText.Text = updateInfo.Version.ToString();
            if (new Version(updateInfo.Version) > currVersion)
            {
                mainWindow.UpdateLink.Visibility = Visibility.Visible;
                mainWindow.UpdateLinkHyperLink.NavigateUri = new Uri(updateInfo.Url);
            }
            else
                mainWindow.CheckUpdateLink.Visibility = Visibility.Visible;
        }

        public static void LoadButtonsOrienationComboBox(MainWindow mainWindow, Manager manager)
        {
            mainWindow.ButtonsOrientationCombobox.ItemsSource = Enum.GetValues(typeof(OrientationsButtonEnum));
            mainWindow.ButtonsOrientationCombobox.SelectedItem = manager.settingsManager.ButtonsOrientation;
        }

        public static void PopolateThemeCombo(MainWindow mainWindow, string theme)
        {
            mainWindow.ThemeBaseSelector.ItemsSource = ThemeManager.Current.Themes.Select(e => e.BaseColorScheme).OrderBy(e => e).Distinct().ToList();
            mainWindow.ColorsSelector.ItemsSource = ThemeManager.Current.Themes.Select(e => e.ColorScheme).OrderBy(e => e).Distinct().ToList();

            if (theme != null)
            {
                SetTheme(mainWindow, theme);
                mainWindow.ThemeBaseSelector.SelectedItem = theme.Split('.')[0];
                mainWindow.ColorsSelector.SelectedItem = theme.Split('.')[1];
            }
            else
            {
                mainWindow.ThemeBaseSelector.SelectedItem = "Light";
                mainWindow.ColorsSelector.SelectedItem = "Cobalt";
            }

        }

        public static void SetTheme(MainWindow mainWindow, string theme)
        {
            ThemeManager.Current.ChangeTheme(mainWindow, theme);
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
