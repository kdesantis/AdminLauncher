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

            List<GenericItem> genericItems = GetSortedGenericItems(programManager);

            foreach (var item in genericItems)
            {
                Button button = CreateButton(item, programManager, mainWindow);
                mainWindow.ButtonPanel.Children.Add(button);
            }
        }

        static List<GenericItem> GetSortedGenericItems(ProgramManager programManager)
        {
            return
            [
                .. programManager.Routines.OrderBy(e => e.Name),
                .. programManager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite),
            ];
        }

        static Button CreateButton(GenericItem item, ProgramManager programManager, MainWindow mainWindow)
        {
            Button button = new Button
            {
                Margin = new Thickness(5),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                Content = CreateButtonContent(item),
                ContextMenu = CreateContextMenu(item, programManager, mainWindow)
            };

            button.Click += (sender, e) => item.Launch();

            return button;
        }

        static UIElement CreateButtonContent(GenericItem item)
        {
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

            if (item is ProgramItem programItem && programItem.IsFavorite)
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

            return grid;
        }

        static ContextMenu CreateContextMenu(GenericItem item, ProgramManager programManager, MainWindow mainWindow)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem deleteMenuItem = new MenuItem { Header = item is ProgramItem ? "Delete Program" : "Delete Routine" };
            deleteMenuItem.Click += (s, e) => OnDeleteClicked(item, programManager, mainWindow);
            contextMenu.Items.Add(deleteMenuItem);

            MenuItem editMenuItem = new MenuItem { Header = item is ProgramItem ? "Edit Program" : "Edit Routine" };
            editMenuItem.Click += (s, e) => OnEditClicked(item, mainWindow, programManager);
            contextMenu.Items.Add(editMenuItem);

            return contextMenu;
        }

        static void OnDeleteClicked(GenericItem item, ProgramManager programManager, MainWindow mainWindow)
        {
            if (ConfirmDeletion(item.Name))
            {
                RemoveItem(item, programManager);
                programManager.Save();
                GenerateButtons(programManager, mainWindow);
            }
        }

        static bool ConfirmDeletion(string itemName)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {itemName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            return result == MessageBoxResult.Yes;
        }

        static void RemoveItem(GenericItem item, ProgramManager programManager)
        {
            if (item is ProgramItem)
            {
                programManager.RemoveProgram((ProgramItem)item);
            }
            else if (item is RoutineItem)
            {
                programManager.RemoveRoutine((RoutineItem)item);
            }
        }
        static void OnEditClicked(GenericItem item, MainWindow mainWindow, ProgramManager programManager)
        {
            if (item is ProgramItem programItem)
            {
                EditProgram(programItem, mainWindow);
            }
            else if (item is RoutineItem routineItem)
            {
                EditRoutine(routineItem, mainWindow, programManager);
            }
        }

        static void EditProgram(ProgramItem program, MainWindow mainWindow)
        {
            InterfaceUtility.InterfaceLoader(InterfaceEnum.AddProgramInterface, mainWindow);
            mainWindow.ProgramIndexLabel.Content = program.Index;
            mainWindow.ProgramNameTextBox.Text = program.Name;
            mainWindow.ProgramPathTextBox.Text = program.Path;
            mainWindow.ProgramArgumentsTextBox.Text = program.Arguments;
            mainWindow.FavoriteCheckBox.IsChecked = program.IsFavorite;
        }

        static void EditRoutine(RoutineItem routine, MainWindow mainWindow, ProgramManager programManager)
        {
            InterfaceUtility.LoadProgramsListBox(programManager, mainWindow);
            InterfaceUtility.InterfaceLoader(InterfaceEnum.AddRoutineInterface, mainWindow);
            mainWindow.RoutineIndexLabel.Content = routine.Index;
            mainWindow.RoutineNameTextBox.Text = routine.Name;
        }
    }
}
