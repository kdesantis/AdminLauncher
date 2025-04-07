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
using NLog;
using AdminLauncher.UpdateLibrary;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Input;
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
        public DialogUtility CurrentDialogUtility;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private ReleaseInformation updateInformation;
        public ObservableCollection<ProgramItemForListbox> ProgramList { get; set; }
        public ObservableCollection<ProgramItemForListbox> FilteredProgramList { get; set; }
        public List<ProgramItem> SelectedProgram { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            LoadInstalledProgramList();
        }

        private void LoadInstalledProgramList()
        {
            DataContext = this;

            // Retrieves the installed programs and creates the ProgramItemForListbox list
            var ProgramsInstalled = new InstalledProgramUtility().GetInstalledProgram();
            ProgramList = new ObservableCollection<ProgramItemForListbox>(
                ProgramsInstalled.Select(p => new ProgramItemForListbox
                {
                    Program = p,
                    IsChecked = false
                })
            );
            ProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
            FilteredProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartOperation(sender, e);
        }

        private void ProcessSelectedPrograms(object sender, RoutedEventArgs e)
        {
            // Filter selected programs
            SelectedProgram = ProgramList.Where(p => p.IsChecked).Select(p => p.Program).ToList();

            foreach (var item in SelectedProgram)
                manager.programManager.AddProgram(item);

            ProgramList.Where(p => p.IsChecked).ToList().ForEach(p => p.IsChecked = false);

            SearchBox.Text = string.Empty;

            ReloadPrograms();
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = SearchBox.Text.ToLower();
            // Update filtered list
            FilteredProgramList.Clear();
            foreach (var program in ProgramList)
            {
                if (program.Program.Name.ToLower().Contains(filterText))
                {
                    FilteredProgramList.Add(program);
                }
            }
        }
        private async void StartOperation(object sender, RoutedEventArgs e)
        {
            CurrentDialogUtility = new(this);
#if !(DEBUG)
            CheckExistsOtherSession();
            IconUtility.DeleteTempIcon();
#endif
            InterfaceControl.PositionWindowInBottomRight(this);
            string backupPath;
            if (!manager.Load(out backupPath))
            {
                CurrentDialogUtility.LoadFailure(backupPath);
            }

            InterfaceControl.PopolateThemeCombo(this, manager.settingsManager.Theme);

            buttonGenerator = new(manager, this);
            notifyIconUtility = new(this, manager);
            InitialPathTextBox.Text = manager.settingsManager.InitialFileDialogPath;
            ReloadPrograms();
#if DEBUG
            ProgramIndexLabel.Visibility = Visibility.Visible;
            RoutineIndexLabel.Visibility = Visibility.Visible;
#endif
            updateInformation = await UpdateUtility.CheckUpdateAsync(false, CurrentDialogUtility);
            InterfaceControl.UpdateVersionText(updateInformation, this);

            InterfaceControl.LoadButtonsOrienationComboBox(this, manager);
            InterfaceControl.LoadWindowOrienationComboBox(this, manager);
        }

        private void CheckExistsOtherSession()
        {
            const string appUniqueName = "AdminLauncher";
            bool isNewInstance;

            _mutex = new Mutex(true, appUniqueName, out isNewInstance);

            if (!isNewInstance)
            {
                firstClosure = false;
                CurrentDialogUtility.MultipleSessionOfApplication();
                Environment.Exit(0);
                //System.Windows.Application.Current.Shutdown();
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
            _mutex?.ReleaseMutex();
            _mutex = null;
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl && UIOperation)
            {
                TabControl tabControl = (TabControl)sender;
                TabItem selectedTab = (TabItem)tabControl.SelectedItem;

                switch (selectedTab.Name.ToString())
                {
                    case "HomeTab":
                        Home_Click(sender, e);
                        break;
                    case "SettingsTab":
                        Settings_Click(sender, e);
                        break;
                    case "AddProgramTab":
                        AddProgram_Click(sender, e);
                        break;
                    case "AddRoutineTab":
                        AddRoutine_Click(sender, e);
                        break;
                    case "AboutTab":
                        About_Click(sender, e);
                        break;
                }
            }
            UIOperation = true;
        }

        private async void CheckUpdateHyperLinl_Click(object sender, RoutedEventArgs e)
        {
            updateInformation = await UpdateUtility.CheckUpdateAsync(true, CurrentDialogUtility);
            InterfaceControl.UpdateVersionText(updateInformation, this);
        }
        private async void UpdateHyperLink_Click(object sender, RoutedEventArgs e)
        {
            UpdateUtility.LaunchUpdateProcedure(this, updateInformation);
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
                CustomIconPath = RoutineIconPathTextBox.Text.Replace("\"", ""),
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
        private void BrowseProgramIconButton_Click(object sender, RoutedEventArgs e)
        {
            var initialPath = !string.IsNullOrEmpty(ProgramIconPathTextBox.Text)
                ? Path.GetDirectoryName(ProgramIconPathTextBox.Text)
                : manager.settingsManager.InitialFileDialogPath;
            var filePath = DialogUtility.ShowOpenFileDialogForIcon(initialPath);
            if (filePath is not null)
            {
                ProgramIconPathTextBox.Text = filePath;
            }
        }
        private void BrowseRoutineIconButton_Click(object sender, RoutedEventArgs e)
        {
            var initialPath = !string.IsNullOrEmpty(RoutineIconPathTextBox.Text)
                ? Path.GetDirectoryName(RoutineIconPathTextBox.Text)
                : manager.settingsManager.InitialFileDialogPath;
            var filePath = DialogUtility.ShowOpenFileDialogForIcon(initialPath);
            if (filePath is not null)
            {
                RoutineIconPathTextBox.Text = filePath;
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
                IsFavorite = FavoriteCheckBox.IsChecked == true,
                CustomIconPath = ProgramIconPathTextBox.Text.Replace("\"", "")
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
            if (manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Mosaic)
                MosaicPreviewStackPanel.Visibility = Visibility.Visible;
            else if (manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Vertical)
                VerticalPreviewStackPanel.Visibility = Visibility.Visible;
        }
        private void WindowOrientationCombobox_Selected(object sender, RoutedEventArgs e)
        {
            manager.settingsManager.WindowOrientation = (WindowOrientationEnum)WindowOrientationCombobox.SelectedItem;
            ReloadPrograms();
            manager.Save();
            InterfaceControl.SetWindowOrietation(this, manager.settingsManager.WindowOrientation);
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
        private void ThemeBaseSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SelectTheme();

        private void ColorsSelectorOnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
            SelectTheme();

        private void SelectTheme()
        {
            if (ThemeBaseSelector.SelectedItem != null && ColorsSelector.SelectedItem != null)
            {
                var theme = $"{ThemeBaseSelector.SelectedItem.ToString()}.{ColorsSelector.SelectedItem.ToString()}";
                InterfaceControl.SetTheme(this, theme);
                manager.settingsManager.Theme = theme;
                manager.Save();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchTextBox.Text)
                                        ? Visibility.Visible
                                        : Visibility.Collapsed;

            if (!string.IsNullOrEmpty(SearchTextBox.Text))
            {
                var FilteredProgramsManager = manager.Clone() as Manager;
                FilteredProgramsManager.programManager.Programs = manager.programManager.GetFilteredPrograms(SearchTextBox.Text);
                FilteredProgramsManager.programManager.Routines = manager.programManager.GetFilteredRoutines(SearchTextBox.Text);
                if (SearchTextBox.Text.ToLower().Trim().Contains("d0nk3y"))
                {
                    FilteredProgramsManager.programManager = (ProgramManager)manager.programManager.Clone();
                    FilteredProgramsManager.programManager.SetDonkeyAttributes();
                }

                var newButtonGenerator = new ButtonsGenerator(FilteredProgramsManager, this);
                newButtonGenerator.GenerateButtons(true);
            }
            else
            {
                buttonGenerator.GenerateButtons();
            }
        }

        private void DonateTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            string koFiUrl = ConfigurationManager.AppSettings["kofiUrl"];
            Process.Start(new ProcessStartInfo
            {
                FileName = koFiUrl,
                UseShellExecute = true
            });
        }

        private void AddProgramTab_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, this);
        }
    }
}