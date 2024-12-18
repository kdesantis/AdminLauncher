﻿using AdminLauncher.BusinessLibrary;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;

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
        public void GenerateVerticalButtons()
        {
            Window.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems();

            foreach (var item in genericItems)
            {
                Button button = CreateButton(item);
                Window.ButtonPanel.Children.Add(button);
            }
        }

        protected Button CreateButton(GenericItem item)
        {
            Button button = new()
            {
                Margin = new Thickness(5),
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                Content = CreateButtonContent(item),
                ContextMenu = CreateContextMenu(item)
            };

            button.Click += (sender, e) => Window.CurrentDialogUtility.LaunchInformatinError(item.Launch());

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
                    Source = IconUtility.GetBitmapImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"favorite.png")),
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
    }
}
