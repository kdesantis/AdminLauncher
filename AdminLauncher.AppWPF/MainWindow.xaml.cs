﻿using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
using AdminLauncher.UpdateLibrary;
using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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
        readonly ProgramManager ProgramManager = new();
        readonly ButtonGenerator buttonGenerator = new();
        public MainWindow()
        {
            InitializeComponent();

            InterfaceControl.PositionWindowInBottomRight(this);
            ProgramManager.Load();
            buttonGenerator.GenerateButtons(ProgramManager, this);
            NotifyIcon notifyIcon = NotifyIconUtility.InitializeNotifyIcon(this);

#if DEBUG
            ProgramIndexLabel.Visibility = Visibility.Visible;
            RoutineIndexLabel.Visibility = Visibility.Visible;
#endif
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var updateInformation = await UpdateUtility.CheckUpdateAsync(false);
            InterfaceControl.UpdateVersionText(updateInformation, this);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private async void CheckUpdateHyperLinl_Click(object sender, RoutedEventArgs e)
        {
            var updateInformation = await UpdateUtility.CheckUpdateAsync(true);
            InterfaceControl.UpdateVersionText(updateInformation, this);
        }
        private void Home_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void AddProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, this);

        private void AddRoutine_Click(object sender, RoutedEventArgs e)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, this);
            InterfaceControl.LoadProgramsListBox(ProgramManager.Programs, this);
        }
        private void QuickRun_Click(object sender, RoutedEventArgs e)
        {
            var filePath = DialogUtility.ShowOpenFileDialog();
            if (filePath is not null)
            {
                var result = (new ProgramItem { Name = "Quick Run", ExecutablePath = filePath }).Launch();
                DialogUtility.LaunchInformatinError(result);
            }
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

            foreach (var selectedProgram in ProgramsListBox.SelectedItems)
            {
                var program = ProgramManager.FindProgramByName(selectedProgram.ToString());
                if (program != null)
                    newRoutine.AddProgram(program);
            }

            ProgramManager.AddRoutine(newRoutine);
            ProgramManager.Save();

            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
            buttonGenerator.GenerateButtons(ProgramManager, this);
        }
        private void CancelRoutine_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = DialogUtility.ShowOpenFileDialog();
            if (filePath is not null)
                ProgramPathTextBox.Text = filePath;
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

            ProgramManager.AddProgram(newProgram);
            ProgramManager.Save();
            buttonGenerator.GenerateButtons(ProgramManager, this);

            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceControl.InterfaceLoader(InterfaceEnum.Home, this);
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

    }
}