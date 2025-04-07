using AdminLauncher.BusinessLibrary;
using MahApps.Metro.IconPacks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using Orientation = System.Windows.Controls.Orientation;
using System.Drawing;
using System.Diagnostics.Eventing.Reader;



namespace AdminLauncher.AppWPF.Utility
{
    public class MosaicButtonsGenerator : ButtonsGenerator
    {
        public MosaicButtonsGenerator(Manager manager, MainWindow mainWindow) : base(manager, mainWindow)
        {
        }

        public void GenerateHorizontalButtons(bool isFiltered)
        {
            Window.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems(isFiltered);

            WrapPanel gridPanel = new()
            {
                Orientation = Orientation.Horizontal,
                ItemWidth = (400 - 35) / 2,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            foreach (var item in genericItems)
            {
                Button productButton = CreateProductButton(item);
                gridPanel.Children.Add(productButton);
            }

            Window.ButtonPanel.Children.Add(gridPanel);
        }

        protected Button CreateProductButton(GenericItem item)
        {
            Button button = new()
            {
                Margin = new Thickness(5),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                Content = CreateProductGrid(item),
                ToolTip = $"Run {item.Name}",
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
            Grid productGrid = new();

            // Define rows
            productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            productGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Define columns
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Main icon in the center above the text
            Image iconImage = new()
            {
                Source = IconUtility.GetBitmapImageIcon(item.GetIconPath()),
                Width = 32,
                Height = 32,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 5)
            };
            Grid.SetRow(iconImage, 0);
            Grid.SetColumn(iconImage, 0);
            Grid.SetColumnSpan(iconImage, 2);
            productGrid.Children.Add(iconImage);

            // Text in the center of the second line
            TextBlock textBlock = new()
            {
                Text = item.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),

            };
            Grid.SetRow(textBlock, 1);
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 2);
            productGrid.Children.Add(textBlock);

            // Right centered menu button
            Button menuButton = new()
            {
                Content = "⋮",
                Width = 24,
                Height = 24,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(2),
                ToolTip = "Show options",
            };

            // create context menu
            menuButton.ContextMenu = CreateContextMenu(item);
            menuButton.Click += (s, e) =>
            {
                if (menuButton.ContextMenu != null)
                {
                    menuButton.ContextMenu.IsOpen = true;
                    e.Handled = true;
                }
            };

            Grid.SetRow(menuButton, 0);
            Grid.SetColumn(menuButton, 1);
            if (item.Index != -1)
                productGrid.Children.Add(menuButton);

            // Favorite icon on top at left
            if (item is ProgramItem programItem && programItem.IsFavorite)
            {
                Image favoriteIcon = new()
                {
                    Source = IconUtility.GetBitmapImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorite.png")),
                    Width = 24,
                    Height = 24,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    //Margin = new Thickness(3)
                };
                Grid.SetRow(favoriteIcon, 0);
                Grid.SetColumn(favoriteIcon, 0);
                productGrid.Children.Add(favoriteIcon);
            }

            return productGrid;
        }

    }
}
