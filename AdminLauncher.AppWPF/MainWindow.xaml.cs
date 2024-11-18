using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Navigation;
using TabControl = System.Windows.Controls.TabControl;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Manager manager = new();
        private ButtonsGenerator buttonGenerator;
        public NotifyIconUtility notifyIconUtility;
        public bool firstClosure = true;
        private static Mutex _mutex;

        public bool UIOperation = true;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
#else
            CheckExistsOtherSession();
            IconUtility.DeleteTempIcon();
#endif
            InterfaceControl.PositionWindowInBottomRight(this);
            if (!manager.Load())
                DialogUtility.LoadFailure();

            InterfaceControl.PopolateThemeCombo(this, manager.settingsManager.Theme);

            buttonGenerator = new(manager, this);
            notifyIconUtility = new(this, manager);
            InitialPathTextBox.Text = manager.settingsManager.InitialFileDialogPath;
            ReloadPrograms();
#if DEBUG
            ProgramIndexLabel.Visibility = Visibility.Visible;
            RoutineIndexLabel.Visibility = Visibility.Visible;
#endif
        }
        private void CheckExistsOtherSession()
        {
            const string appUniqueName = "AdminLauncher";
            bool isNewInstance;

            _mutex = new Mutex(true, appUniqueName, out isNewInstance);

            if (!isNewInstance)
            {
                firstClosure = false;
                DialogUtility.MultipleSessionOfApplication();
                System.Windows.Application.Current.Shutdown();
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var updateInformation = await UpdateUtility.CheckUpdateAsync(false);
            InterfaceControl.UpdateVersionText(updateInformation, this);

            InterfaceControl.LoadButtonsOrienationComboBox(this, manager);

            if (manager.programManager.Programs.Count < 1) 
            {
                LaunchWizard_Click(sender, e);
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            if (firstClosure)
            {
                DialogUtility.ShowBalloonTipAppHided(notifyIconUtility.AppNotifyIcon);
                firstClosure = false;
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_mutex is not null)
                _mutex?.ReleaseMutex();
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && UIOperation)
            {
                TabControl tabControl = (TabControl)sender;
                TabItem selectedTab = (TabItem)tabControl.SelectedItem;

                switch (selectedTab.Header.ToString())
                {
                    case "Home":
                        Home_Click(sender, e);
                        break;
                    case "Settings":
                        Settings_Click(sender, e);
                        break;
                    case "Add Program":
                        AddProgram_Click(sender, e);
                        break;
                    case "Add Routine":
                        AddRoutine_Click(sender, e);
                        break;
                    case "About":
                        About_Click(sender, e);
                        break;
                }
            }
            UIOperation = true;
        }

        private async void CheckUpdateHyperLinl_Click(object sender, RoutedEventArgs e)
        {
            var updateInformation = await UpdateUtility.CheckUpdateAsync(true);
            InterfaceControl.UpdateVersionText(updateInformation, this);
        }
        private void Home_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void Settings_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Settings, this);
        private void AddProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, this);

        private void AddRoutine_Click(object sender, RoutedEventArgs e)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, this);
            InterfaceControl.LoadProgramsListBox(manager.programManager.Programs, this);
        }
        public void QuickRun_Click(object sender, RoutedEventArgs e)
        {
            QuickRunUtils.LaunchQuickRun(manager.settingsManager.InitialFileDialogPath);
        }
        public void ReloadPrograms()
        {
            buttonGenerator.GenerateButtons();
            notifyIconUtility.AddContextMenu(manager.programManager);
        }
        private void About_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.About, this);
        private void SaveRoutine_Click(object sender, RoutedEventArgs e)
        {
            RoutineItem newRoutine = new()
            {
                Index = Int32.Parse(RoutineIndexLabel.Content.ToString()),
                Name = RoutineNameTextBox.Text,
                Programs = []
            };

            foreach (var selectedProgram in ProgramsListBox.ItemsSource as IEnumerable<ProgramItemForListbox>)
            {
                if (selectedProgram.IsChecked)
                    newRoutine.AddProgram(selectedProgram.Program);
            }

            manager.programManager.AddRoutine(newRoutine);
            manager.Save();

            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
            ReloadPrograms();
        }
        private void CancelRoutine_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = DialogUtility.ShowOpenFileDialog(manager.settingsManager.InitialFileDialogPath);
            if (filePath is not null)
            {
                ProgramPathTextBox.Text = filePath;
                if (string.IsNullOrEmpty(ProgramNameTextBox.Text))
                    ProgramNameTextBox.Text = Path.GetFileNameWithoutExtension(filePath);
            }
        }
        private void SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            ProgramItem newProgram = new()
            {
                Index = Int32.Parse(ProgramIndexLabel.Content.ToString()),
                Name = ProgramNameTextBox.Text,
                ExecutablePath = ProgramPathTextBox.Text,
                Arguments = ProgramArgumentsTextBox.Text,
                IsFavorite = FavoriteCheckBox.IsChecked == true
            };

            manager.programManager.AddProgram(newProgram);
            manager.Save();
            ReloadPrograms();

            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void ButtonsOrientationCombobox_Selected(object sender, RoutedEventArgs e)
        {
            manager.settingsManager.ButtonsOrientation = (OrientationsButtonEnum)ButtonsOrientationCombobox.SelectedItem;
            ReloadPrograms();
            manager.Save();
            
            MosaicPreviewStackPanel.Visibility = Visibility.Collapsed;
            VerticalPreviewStackPanel.Visibility = Visibility.Collapsed;
            if(manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Mosaic)
                MosaicPreviewStackPanel.Visibility = Visibility.Visible;
            else if (manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Vertical)
                VerticalPreviewStackPanel.Visibility = Visibility.Visible;
        }

        private void InitialPathButton_Click(object sender, RoutedEventArgs e)
        {
            var directoryPath = DialogUtility.ShowOpenFolderDialog();
            if (directoryPath is not null)
            {
                InitialPathTextBox.Text = directoryPath;
                manager.settingsManager.InitialFileDialogPath = directoryPath;
                manager.Save();
            }
        }
        private void EraseInitialPath_Click(object sender, RoutedEventArgs e)
        {
            InitialPathTextBox.Text = string.Empty;
            manager.settingsManager.InitialFileDialogPath = null;
            manager.Save();
        }
        private void KoFi_Click(object sender, RoutedEventArgs e)
        {
            string koFiUrl = ConfigurationManager.AppSettings["kofiUrl"];
            Process.Start(new ProcessStartInfo
            {
                FileName = koFiUrl,
                UseShellExecute = true
            });
        }
        private void ThemeBaseSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeBaseSelector.SelectedItem != null && ColorsSelector.SelectedItem != null)
            {
                var theme = $"{ThemeBaseSelector.SelectedItem.ToString()}.{ColorsSelector.SelectedItem.ToString()}";
                InterfaceControl.SetTheme(this, theme);
                manager.settingsManager.Theme = theme;
                manager.Save();
            }
        }

        private void ColorsSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeBaseSelector.SelectedItem != null && ColorsSelector.SelectedItem != null)
            {
                var theme = $"{ThemeBaseSelector.SelectedItem.ToString()}.{ColorsSelector.SelectedItem.ToString()}";
                InterfaceControl.SetTheme(this, theme);
                manager.settingsManager.Theme = theme;
                manager.Save();
            }
        }

        private void LaunchWizard_Click(object sender, RoutedEventArgs e)
        {
            ProgramsConfiguratorWizard secondWindow = new(manager.programManager.Programs, manager.settingsManager.Theme);

            var result = secondWindow.ShowDialog();
            if (result == true)
            {
                List<ProgramItem> selectedPrograms = secondWindow.SelectedProgram;
                foreach (var program in selectedPrograms)
                {
                    if(!manager.programManager.Programs.Any(e => e.ExecutablePath == program.ExecutablePath && e.Arguments == program.Arguments))
                    {
                        program.Index = manager.programManager.CurrIndex;
                        manager.programManager.AddProgram(program);
                    }
                    manager.Save();
                    ReloadPrograms();
                }
            }
        }
    }
}