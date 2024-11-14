using AdminLauncher.BusinessLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using Orientation = System.Windows.Controls.Orientation;



namespace AdminLauncher.AppWPF.Utility
{
    public class MosaicButtonsGenerator : ButtonsGenerator
    {
        public MosaicButtonsGenerator(Manager manager, MainWindow mainWindow) : base(manager, mainWindow)
        {
        }

        public void GenerateHorizontalButtons()
        {
            Window.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems();

            WrapPanel gridPanel = new()
            {
                Orientation = Orientation.Horizontal,
                ItemWidth = (Window.Width - 35) / 2,
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
                ContextMenu = CreateContextMenu(item)
            };

            button.Click += (sender, e) => DialogUtility.LaunchInformatinError(item.Launch());

            return button;
        }

        private static Grid CreateProductGrid(GenericItem item)
        {
            Grid productGrid = new();

            RowDefinition iconRow = new() { Height = GridLength.Auto };
            productGrid.RowDefinitions.Add(iconRow);

            RowDefinition nameRow = new() { Height = GridLength.Auto };
            productGrid.RowDefinitions.Add(nameRow);

            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

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

            TextBlock textBlock = new()
            {
                Text = item.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0) 
            };
            Grid.SetRow(textBlock, 1); 
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 2); 
            productGrid.Children.Add(textBlock);

            if (item is ProgramItem programItem && programItem.IsFavorite)
            {
                Image favoriteIcon = new()
                {
                    Source = IconUtility.GetBitmapImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorite.png")),
                    Width = 24,
                    Height = 24,
                    HorizontalAlignment = HorizontalAlignment.Right, 
                    VerticalAlignment = VerticalAlignment.Top 
                };
                Grid.SetRow(favoriteIcon, 0); 
                Grid.SetColumn(favoriteIcon, 1); 
                productGrid.Children.Add(favoriteIcon);
            }

            return productGrid;
        }


    }
}
