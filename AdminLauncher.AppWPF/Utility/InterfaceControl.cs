using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdminLauncher.AppWPF.Utility
{
    public static class InterfaceControl
    {
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
        public static void InterfaceLoader(InterfaceEnum mode, MainWindow mainWindow)
        {
            switch (mode)
            {
                case InterfaceEnum.Home:
                    mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
                    mainWindow.MainScrollViewer.Visibility = Visibility.Visible;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
                    mainWindow.AbountPanel.Visibility = Visibility.Collapsed;
                    ClearAddProgramData(mainWindow);
                    ClearRoutineData(mainWindow);
                    break;
                case InterfaceEnum.AddProgramInterface:
                    mainWindow.AddProgramPanel.Visibility = Visibility.Visible;
                    mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
                    mainWindow.AbountPanel.Visibility = Visibility.Collapsed;
                    break;
                case InterfaceEnum.AddRoutineInterface:
                    mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
                    mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Visible;
                    mainWindow.AbountPanel.Visibility = Visibility.Collapsed;
                    break;
                case InterfaceEnum.About:
                    mainWindow.MainScrollViewer.Visibility = Visibility.Collapsed;
                    mainWindow.AddProgramPanel.Visibility = Visibility.Collapsed;
                    mainWindow.AddRoutinePanel.Visibility = Visibility.Collapsed;
                    mainWindow.AbountPanel.Visibility = Visibility.Visible;

                    break;
            }
        }
        public static void LoadProgramsListBox(List<ProgramItem> programs, MainWindow mainWindow)
        {
            mainWindow.ProgramsListBox.Items.Clear();
            foreach (var program in programs.OrderBy(e => e.Name))
                mainWindow.ProgramsListBox.Items.Add(program.Name);
        }
    }
    public enum InterfaceEnum
    {
        Home,
        AddProgramInterface,
        AddRoutineInterface,
        About
    }
}
