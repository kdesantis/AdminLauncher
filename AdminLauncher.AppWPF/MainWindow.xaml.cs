using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgramManager ProgramManager = new();
        private NotifyIcon notifyIcon;
        Version currVersion = new Version("0.0.1");
        public MainWindow()
        {
            InitializeComponent();

            InterfaceControl.PositionWindowInBottomRight(this);
            ProgramManager.Load();
            ButtonGenerator.GenerateButtons(ProgramManager, this);
            notifyIcon = NotifyIconUtility.InitializeNotifyIcon(this);
            UpdateUtility.CheckUpdateAsync(currVersion);
#if DEBUG
            ProgramIndexLabel.Visibility = Visibility.Visible;
            RoutineIndexLabel.Visibility = Visibility.Visible;
#endif
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void AddProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, this);

        private void AddRoutine_Click(object sender, RoutedEventArgs e)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, this);
            InterfaceControl.LoadProgramsListBox(ProgramManager.Programs, this);
        }
        private void QuickRun_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                new ProgramItem { Path = openFileDialog.FileName }.Launch();
        }

        private void SaveRoutine_Click(object sender, RoutedEventArgs e)
        {
            RoutineItem newRoutine = new RoutineItem
            {
                Index = Int32.Parse(RoutineIndexLabel.Content.ToString()),
                Name = RoutineNameTextBox.Text,
                Programs = new List<ProgramItem>()
            };

            foreach (var selectedProgram in ProgramsListBox.SelectedItems)
            {
                var program = ProgramManager.FindProgramByName(selectedProgram.ToString());
                if (program != null)
                    newRoutine.AddProgram(program);
            }

            ProgramManager.AddRoutine(newRoutine);
            ProgramManager.Save();

            InterfaceControl.InterfaceLoader(InterfaceEnum.MainInterface, this);
            ButtonGenerator.GenerateButtons(ProgramManager, this);
        }
        private void CancelRoutine_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.MainInterface, this);
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                ProgramPathTextBox.Text = openFileDialog.FileName;
        }
        private void SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            ProgramItem newProgram = new ProgramItem
            {
                Index = Int32.Parse(ProgramIndexLabel.Content.ToString()),
                Name = ProgramNameTextBox.Text,
                Path = ProgramPathTextBox.Text,
                Arguments = ProgramArgumentsTextBox.Text,
                IsFavorite = FavoriteCheckBox.IsChecked == true
            };

            ProgramManager.AddProgram(newProgram);
            ProgramManager.Save();
            ButtonGenerator.GenerateButtons(ProgramManager, this);

            InterfaceControl.InterfaceLoader(InterfaceEnum.MainInterface, this);
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.MainInterface, this);
    }
}