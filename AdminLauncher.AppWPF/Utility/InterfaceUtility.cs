using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminLauncher.AppWPF.Utility
{
    public static class InterfaceUtility
    {
        public static void PositionWindowInBottomRight(MainWindow mainWindow)
        {
            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaWidth = SystemParameters.WorkArea.Width;

            mainWindow.Left = workAreaWidth - mainWindow.Width;
            mainWindow.Top = workAreaHeight - mainWindow.Height;
        }
        public static void ClearAddProgramData(MainWindow mainWindow)
        {
            mainWindow.ProgramIndexLabel.Content = -1;
            mainWindow.ProgramNameTextBox.Clear();
            mainWindow.ProgramPathTextBox.Clear();
            mainWindow.ProgramArgumentsTextBox.Clear();
            mainWindow.FavoriteCheckBox.IsChecked = false;
        }
        public static void ClearRoutineData(MainWindow mainWindow)
        {
            mainWindow.RoutineIndexLabel.Content = -1;
            mainWindow.RoutineNameTextBox.Clear();
            mainWindow.ProgramsListBox.UnselectAll();
        }
        public static void InterfaceLoader(InterfaceEnum mode, MainWindow mainWindow)
        {
            switch (mode)
            {
                case InterfaceEnum.MainInterface:
                    mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
                    mainWindow.MainScrollViewer.Visibility = Visibility.Visible;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
                    InterfaceUtility.ClearAddProgramData(mainWindow);
                    InterfaceUtility.ClearRoutineData(mainWindow);
                    break;
                case InterfaceEnum.AddProgramInterface:
                    mainWindow.AddProgramPanel.Visibility = Visibility.Visible;
                    mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
                    break;
                case InterfaceEnum.AddRoutineInterface:
                    mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
                    mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Visible;
                    break;
            }
        }
        public static void LoadProgramsListBox(List<ProgramItem> programs, MainWindow mainWindow)
        {
            mainWindow.ProgramsListBox.Items.Clear();
            foreach (var program in programs)
                mainWindow.ProgramsListBox.Items.Add(program.Name);
        }
    }
}
