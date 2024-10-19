using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manager manager = new();
        private ButtonsGenerator buttonGenerator;
        public NotifyIconUtility notifyIconUtility;
        public bool firstClosure = true;
        private static Mutex _mutex;
        public MainWindow()
        {
            InitializeComponent();

            IconUtility.DeleteTempIcon();
#if DEBUG
#else
            CheckExistsOtherSession();
#endif

            InterfaceControl.PositionWindowInBottomRight(this);

            if (!manager.Load())
                DialogUtility.LoadFailure();

            buttonGenerator = new(manager, this);
            notifyIconUtility = new(this,manager.programManager);
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
            QuickRunUtils.LaunchQuickRun();
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
            var filePath = DialogUtility.ShowOpenFileDialog();
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
        }
    }
}