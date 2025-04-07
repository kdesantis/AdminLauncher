using AdminLauncher.BusinessLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
namespace AdminLauncher.AppWPF.Utility
{
    public class VerticalButtonsGenerator : ButtonsGenerator
    {
        public VerticalButtonsGenerator(Manager manager, MainWindow mainWindow) : base(manager, mainWindow)
        {
        }

        /// <summary>
        /// Populates the StackPanel “ButtonPanel” of the MainWindows with all the programs and routines in the program manager
        /// </summary>
        /// <param name="programManager"></param>
        /// <param name="mainWindow"></param>
        public void GenerateVerticalButtons(bool isFiltered)
        {
            Window.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems(isFiltered);

            foreach (var item in genericItems)
            {
                Button button = CreateProductButton(item);
                Window.ButtonPanel.Children.Add(button);
            }
        }

        protected Button CreateProductButton(GenericItem item)
        {
            Button button = new()
            {
                Margin = new Thickness(5),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                Content = CreateProductGrid(item),
               
            };
            if (item.Index == -1)
            {
                button.ToolTip = $"Select a program to run";
                button.Click += (sender, e) => QuickRunUtils.LaunchQuickRun(Manager.settingsManager.InitialFileDialogPath, Window.CurrentDialogUtility);
            }
            else
            {
                button.ToolTip = $"Run {item.Name}";
                button.Click += (sender, e) => Window.CurrentDialogUtility.LaunchInformatinError(item.Launch());
            }

            return button;
        }

        private Grid CreateProductGrid(GenericItem item)
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Image iconImage = new()
            {
                Source = IconUtility.GetBitmapImageIcon(item.GetIconPath()),
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
                    Source = IconUtility.GetBitmapImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorite.png")),
                    Width = 24,
                    Height = 24,
                    Margin = new Thickness(0, 0, 30, 0),
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,

                };
                Grid.SetColumn(favoriteIcon, 2);
                grid.Children.Add(favoriteIcon);
            }

            // button for open the menu
            Button menuButton = new()
            {
                Content = "⋮",
                Width = 24,
                Height = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(2),
                ToolTip = "Show options",
            };

            // Creazione del ContextMenu
            ContextMenu contextMenu = CreateContextMenu(item);
            menuButton.ContextMenu = contextMenu;

            // Show menu when click the button
            menuButton.Click += (s, e) =>
            {
                if (contextMenu != null)
                {
                    contextMenu.PlacementTarget = menuButton;
                    contextMenu.IsOpen = true;
                    e.Handled = true; // Prevents the event from spreading
                }
            };

            Grid.SetColumn(menuButton, 3);
            if (item.Index != -1)
                grid.Children.Add(menuButton);

            return grid;
        }

    }
}
