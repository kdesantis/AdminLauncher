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
        private static ProgramManager? CurrentProgramManager;
        private static MainWindow? CurrentMainWindows;
        /// <summary>
        /// Populates the StackPanel “ButtonPanel” of the MainWindows with all the programs and routines in the program manager
        /// </summary>
        /// <param name="programManager"></param>
        /// <param name="mainWindow"></param>
        public static void GenerateButtons(ProgramManager programManager, MainWindow mainWindow)
        {
            CurrentProgramManager = programManager;
            CurrentMainWindows = mainWindow;

            CurrentMainWindows.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems();

            foreach (var item in genericItems)
            {
                Button button = CreateButton(item);
                CurrentMainWindows.ButtonPanel.Children.Add(button);
            }
        }

        private static List<GenericItem> GetSortedGenericItems()
        {
            return
            [
                .. CurrentProgramManager.Routines.OrderBy(e => e.Name),
                .. CurrentProgramManager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite),
            ];
        }

        private static Button CreateButton(GenericItem item)
        {
            Button button = new()
            {
                Margin = new Thickness(5),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                Content = CreateButtonContent(item),
                ContextMenu = CreateContextMenu(item)
            };

            button.Click += (sender, e) => DialogUtility.LaunchInformatinError(item.Launch());

            return button;
        }

        private static Grid CreateButtonContent(GenericItem item)
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Image iconImage = new()
            {
                Source = IconUtility.LoadIcon(item.GetIconPath()),
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 5, 0)
            };
            Grid.SetColumn(iconImage, 0);
            grid.Children.Add(iconImage);

            TextBlock textBlock = new()
            {
                Text = item.Name,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(textBlock, 1);
            grid.Children.Add(textBlock);

            if (item is ProgramItem programItem && programItem.IsFavorite)
            {
                Image favoriteIcon = new()
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

        private static ContextMenu CreateContextMenu(GenericItem item)
        {
            ContextMenu contextMenu = new();

            MenuItem editMenuItem = new() { Header = item is ProgramItem ? "Edit Program" : "Edit Routine" };
            editMenuItem.Click += (s, e) => OnEditClicked(item);
            contextMenu.Items.Add(editMenuItem);

            MenuItem deleteMenuItem = new() { Header = item is ProgramItem ? "Delete Program" : "Delete Routine" };
            deleteMenuItem.Click += (s, e) => OnDeleteClicked(item);
            contextMenu.Items.Add(deleteMenuItem);

            return contextMenu;
        }

        private static void OnDeleteClicked(GenericItem item)
        {
            if (ConfirmDeletion(item.Name))
            {
                RemoveItem(item);
                CurrentProgramManager.Save();
                GenerateButtons(CurrentProgramManager, CurrentMainWindows);
            }
        }

        private static bool ConfirmDeletion(string itemName)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {itemName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            return result == MessageBoxResult.Yes;
        }

        private static void RemoveItem(GenericItem item)
        {
            if (item is ProgramItem program)
                CurrentProgramManager.RemoveProgram(program);
            else if (item is RoutineItem routine)
                CurrentProgramManager.RemoveRoutine(routine);
        }
        private static void OnEditClicked(GenericItem item)
        {
            if (item is ProgramItem programItem)
                EditProgram(programItem);
            else if (item is RoutineItem routineItem)
                EditRoutine(routineItem);
        }

        private static void EditProgram(ProgramItem program)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, CurrentMainWindows);
            CurrentMainWindows.ProgramIndexLabel.Content = program.Index;
            CurrentMainWindows.ProgramNameTextBox.Text = program.Name;
            CurrentMainWindows.ProgramPathTextBox.Text = program.ExecutablePath;
            CurrentMainWindows.ProgramArgumentsTextBox.Text = program.Arguments;
            CurrentMainWindows.FavoriteCheckBox.IsChecked = program.IsFavorite;
        }

        private static void EditRoutine(RoutineItem routine)
        {
            InterfaceControl.LoadProgramsListBox(CurrentProgramManager.Programs, CurrentMainWindows, routine);
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, CurrentMainWindows);
            CurrentMainWindows.RoutineIndexLabel.Content = routine.Index;
            CurrentMainWindows.RoutineNameTextBox.Text = routine.Name;
        }
    }
}
