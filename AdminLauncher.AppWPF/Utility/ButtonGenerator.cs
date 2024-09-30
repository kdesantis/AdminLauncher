using AdminLauncher.BusinessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using System.IO;

namespace AdminLauncher.AppWPF.Utility
{
    public static class ButtonGenerator
    {
        public static void GenerateButtons(ProgramManager programManager, MainWindow mainWindow)
        {
            mainWindow.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems =
            [
                .. programManager.Routines.OrderBy(e => e.Name),
                .. programManager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite),
            ];

            foreach (var item in genericItems)
            {
                Button button = new Button
                {
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                };

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                Image iconImage = new Image
                {
                    Source = IconUtility.LoadIcon(item.GetIconPath()),
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                Grid.SetColumn(iconImage, 0);
                grid.Children.Add(iconImage);

                TextBlock textBlock = new TextBlock
                {
                    Text = item.Name,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(textBlock, 1);
                grid.Children.Add(textBlock);

                if (item is ProgramItem && ((ProgramItem)item).IsFavorite)
                {
                    Image favoriteIcon = new Image
                    {
                        Source = IconUtility.LoadIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorite.png")),
                        Width = 32,
                        Height = 32,
                        Margin = new Thickness(0, 0, 5, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Right
                    };
                    Grid.SetColumn(favoriteIcon, 2);
                    grid.Children.Add(favoriteIcon);
                }

                button.Content = grid;

                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem { Header = item is ProgramItem ? "Delete Program" : "Delete Routine" };
                deleteMenuItem.Click += (s, e) => OnDeleteClicked(item, programManager, mainWindow);
                contextMenu.Items.Add(deleteMenuItem);

                MenuItem editMenuItem = new MenuItem { Header = item is ProgramItem ? "Edit Program" : "Edit Routine" };
                editMenuItem.Click += (s, e) => OnEditClicked(item, programManager, mainWindow);
                contextMenu.Items.Add(editMenuItem);
                button.ContextMenu = contextMenu;

                button.Click += (sender, e) => item.Launch();

                mainWindow.ButtonPanel.Children.Add(button);
            }
        }
        static void OnDeleteClicked(GenericItem item, ProgramManager programManager, MainWindow mainWindow)
        {
            MessageBoxResult result = MessageBox.Show($"Can I leave the case here? Are you sure you want to delete {item.Name}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (item is ProgramItem)
                    programManager.RemoveProgram((ProgramItem)item);
                else
                    programManager.RemoveRoutine((RoutineItem)item);
                programManager.Save();
            }
            GenerateButtons(programManager, mainWindow);
        }
        static void OnEditClicked(GenericItem item, ProgramManager programManager, MainWindow mainWindow)
        {
            if (item is ProgramItem)
            {
                var program = (ProgramItem)item;
                InterfaceUtility.InterfaceLoader(InterfaceEnum.AddProgramInterface, mainWindow);
                mainWindow.ProgramIndexLabel.Content = program.Index;
                mainWindow.ProgramNameTextBox.Text = program.Name;
                mainWindow.ProgramPathTextBox.Text = program.Path;
                mainWindow.ProgramArgumentsTextBox.Text = program.Arguments;
                mainWindow.FavoriteCheckBox.IsChecked = program.IsFavorite;

            }
            else if (item is RoutineItem)
            {
                LoadProgramsListBox(programManager, mainWindow);
                var routine = (RoutineItem)item;
                mainWindow.RoutineIndexLabel.Content = routine.Index;
                InterfaceUtility.InterfaceLoader(InterfaceEnum.AddRoutineInterface, mainWindow);
                mainWindow.RoutineNameTextBox.Text = routine.Name;
            }
        }
        public static void LoadProgramsListBox(ProgramManager programManager, MainWindow mainWindow)
        {
            mainWindow.ProgramsListBox.Items.Clear();
            foreach (var program in programManager.Programs)
                mainWindow.ProgramsListBox.Items.Add(program.Name);
        }
    }
}
