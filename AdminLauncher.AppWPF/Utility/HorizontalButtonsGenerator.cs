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
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Orientation = System.Windows.Controls.Orientation;



namespace AdminLauncher.AppWPF.Utility
{
    public class HorizontalButtonsGenerator : ButtonGenerator
    {

        public void GenerateHorizontalButtons(ProgramManager programManager, MainWindow mainWindow)
        {
            Manager = programManager;
            Window = mainWindow;

            Window.ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = GetSortedGenericItems();

            // Creiamo un WrapPanel per gestire la griglia con due colonne
            WrapPanel gridPanel = new()
            {
                Orientation = Orientation.Horizontal,
                ItemWidth = (mainWindow.Width - 20) / 2,
                ItemHeight = 150,
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            foreach (var item in genericItems)
            {
                // Crea un pulsante con il layout desiderato
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
            // Griglia per ogni prodotto con 2 righe: icona e nome
            Grid productGrid = new();

            // Prima riga: aggiungiamo l'icona del prodotto
            RowDefinition iconRow = new() { Height = GridLength.Auto };
            productGrid.RowDefinitions.Add(iconRow);

            // Seconda riga: aggiungiamo il nome del prodotto
            RowDefinition nameRow = new() { Height = GridLength.Auto };
            productGrid.RowDefinitions.Add(nameRow);

            // Colonna per l'icona del preferito (in alto a destra)
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Immagine dell'icona
            Image iconImage = new()
            {
                Source = IconUtility.LoadIcon(item.GetIconPath()),
                Width = 64, // Dimensione dell'icona aumentata
                Height = 64,
                HorizontalAlignment = HorizontalAlignment.Center, // Centra l'icona orizzontalmente
                Margin = new Thickness(0, 5, 0, 5) // Margini sopra e sotto
            };
            Grid.SetRow(iconImage, 0); // Prima riga
            Grid.SetColumn(iconImage, 0); // Colonna principale
            Grid.SetColumnSpan(iconImage, 2); // Span su due colonne per essere centrato
            productGrid.Children.Add(iconImage);

            // Nome del prodotto
            TextBlock textBlock = new()
            {
                Text = item.Name,
                HorizontalAlignment = HorizontalAlignment.Center, // Centra il testo orizzontalmente
                Margin = new Thickness(0, 5, 0, 0) // Margine superiore per distanziare dal bordo
            };
            Grid.SetRow(textBlock, 1); // Seconda riga
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 2); // Span su due colonne per essere centrato
            productGrid.Children.Add(textBlock);

            // Icona del preferito (se è un preferito)
            if (item is ProgramItem programItem && programItem.IsFavorite)
            {
                Image favoriteIcon = new()
                {
                    Source = IconUtility.LoadIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favorite.png")),
                    Width = 24,
                    Height = 24,
                    HorizontalAlignment = HorizontalAlignment.Right, // Allineato a destra
                    VerticalAlignment = VerticalAlignment.Top // In alto a destra
                };
                Grid.SetRow(favoriteIcon, 0); // Nella prima riga con l'icona principale
                Grid.SetColumn(favoriteIcon, 1); // In alto a destra (seconda colonna)
                productGrid.Children.Add(favoriteIcon);
            }

            return productGrid;
        }


    }
}
