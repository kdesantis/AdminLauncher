using AdminLauncher.BusinessLibrary;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgramManager ProgramManager = new();
        public MainWindow()
        {
            InitializeComponent();

            PositionWindowInBottomRight();

            ProgramManager.Load();

            CreateButtons();

        }
        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelectorMode(true);
        }
        private void SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            // Crea un nuovo oggetto ProgramItem
            ProgramItem newProgram = new ProgramItem
            {
                Name = ProgramNameTextBox.Text,
                Path = ProgramPathTextBox.Text,
                Arguments = ProgramArgumentsTextBox.Text,
                IsFavorite = FavoriteCheckBox.IsChecked == true
            };

            // Aggiungi il ProgramItem a ProgramsManager e salva
            ProgramManager.AddProgram(newProgram);
            ProgramManager.Save();
            CreateButtons();

            // Torna alla vista principale
            InterfaceSelectorMode(false);
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ProgramPathTextBox.Text = openFileDialog.FileName;
            }
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelectorMode(false);

            // (Facoltativo) Pulizia dei campi della form
            ProgramNameTextBox.Clear();
            ProgramPathTextBox.Clear();
            ProgramArgumentsTextBox.Clear();
            FavoriteCheckBox.IsChecked = false;
        }
        private void OnDeleteProgramClicked(ProgramItem item)
        {
            MessageBoxResult result = MessageBox.Show($"Can I leave the case here? Are you sure you want to delete {item.Name}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Se l'utente conferma, chiama il metodo RemoveProgram del ProgramManager
                ProgramManager.RemoveProgram(item);
                ProgramManager.Save();
            }
            CreateButtons();
        }

        private void InterfaceSelectorMode(bool addMode)
        {
            if (addMode)
            {
                AddProgramPanel.Visibility = Visibility.Visible;
                MainScrollViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                AddProgramPanel.Visibility = Visibility.Collapsed;
                MainScrollViewer.Visibility = Visibility.Visible;
            }
        }
        private void PositionWindowInBottomRight()
        {
            // Ottieni l'area di lavoro disponibile (esclude la barra delle applicazioni)
            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaWidth = SystemParameters.WorkArea.Width;

            // Imposta la posizione della finestra in basso a destra
            this.Left = workAreaWidth - this.Width;
            this.Top = workAreaHeight - this.Height;
        }
        // Metodo per creare i pulsanti dinamicamente
        private void CreateButtons()
        {
            ButtonPanel.Children.Clear();
            // Assumendo che ci sia uno StackPanel in XAML con il nome "ButtonPanel"
            foreach (var item in ProgramManager.Programs)
            {

                // Crea il pulsante
                Button button = new Button
                {
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };

                // Crea un DockPanel per inserire l'icona e il testo
                DockPanel dockPanel = new DockPanel();

                // Aggiungi l'icona al pulsante
                Image iconImage = new Image
                {
                    Source = LoadIcon(item.GetIconPath()),  // Ottieni l'icona
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 5, 0) // Margine tra icona e testo
                };
                DockPanel.SetDock(iconImage, Dock.Left);  // Posiziona l'icona a sinistra
                dockPanel.Children.Add(iconImage);

                // Aggiungi il nome del programma al pulsante
                TextBlock textBlock = new TextBlock
                {
                    Text = item.Name,
                    VerticalAlignment = VerticalAlignment.Center
                };
                dockPanel.Children.Add(textBlock);
                button.Content = dockPanel;
                // Crea il ContextMenu con l'opzione "Delete Program"
                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem { Header = "Delete Program" };
                deleteMenuItem.Click += (s, e) => OnDeleteProgramClicked(item); // Associa l'evento click
                contextMenu.Items.Add(deleteMenuItem);
                button.ContextMenu = contextMenu;

                // Associa il click del pulsante al metodo specifico dell'oggetto
                button.Click += (sender, e) => item.Launch();

                // Aggiungi il pulsante al pannello
                ButtonPanel.Children.Add(button);
            }
        }
        private static BitmapImage LoadIcon(string iconPath)
        {
            BitmapImage bitmap = new BitmapImage();
            if (iconPath != null)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(iconPath, UriKind.Absolute); // Usa UriKind.Absolute se il percorso è assoluto
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Cache per evitare blocchi di file
                bitmap.EndInit();
            }
            return bitmap;
        }

    }
}