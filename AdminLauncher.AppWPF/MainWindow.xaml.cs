using AdminLauncher.BusinessLibrary;
using Microsoft.Win32;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ProgramManager ProgramManager = new();
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();

            PositionWindowInBottomRight();

            ProgramManager.Load();

            CreateButtons();

            // Inizializza NotifyIcon
            InitializeNotifyIcon();
        }
        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application, // Puoi utilizzare una tua icona
                Visible = true,
                Text = "Admin Launcher"
            };

            // Menu contestuale per NotifyIcon
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Close", null, OnCloseClick);
            notifyIcon.ContextMenuStrip = contextMenu;

            // Evento per ripristinare l'applicazione al doppio clic
            notifyIcon.DoubleClick += (s, e) => ShowWindow();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Annulla la chiusura della finestra
            Hide(); // Nascondi la finestra invece di chiuderla
        }

        private void ShowWindow()
        {
            Show(); // Mostra la finestra
            WindowState = WindowState.Normal; // Assicurati che la finestra non sia minimizzata
        }

        private void OnCloseClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown(); // Chiudi l'applicazione
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon.Dispose(); // Pulisci la risorsa NotifyIcon
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Cancella l'evento di chiusura
            this.Hide(); // Nasconde l'applicazione
        }
        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelectorMode(1);
        }
        // Evento al click di "Add Routine"
        private void AddRoutine_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelectorMode(2);

            // Popola la lista dei ProgramItem nel ListBox
            ProgramsListBox.Items.Clear();
            foreach (var program in ProgramManager.Programs)
            {
                ProgramsListBox.Items.Add(program.Name); // Aggiungiamo il nome del programma
            }
        }
        // Evento al click del bottone "Salva Routine"
        private void SaveRoutine_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nuova RoutineItem
            RoutineItem newRoutine = new RoutineItem
            {
                Name = RoutineNameTextBox.Text,
                Programs = new List<ProgramItem>()
            };

            // Aggiunge i programmi selezionati nella Routine
            foreach (var selectedProgram in ProgramsListBox.SelectedItems)
            {
                var program = ProgramManager.FindProgramByName(selectedProgram.ToString());
                if (program != null)
                {
                    newRoutine.AddProgram(program);
                }
            }

            // Aggiungi la nuova Routine a ProgramManager e salva
            ProgramManager.AddRoutine(newRoutine);
            ProgramManager.Save();

            InterfaceSelectorMode(0);
            CreateButtons();
        }
        // Evento per annullare la creazione della routine
        private void CancelRoutine_Click(object sender, RoutedEventArgs e)
        {
            AddRoutinePanel.Visibility = Visibility.Collapsed;
            MainScrollViewer.Visibility = Visibility.Visible;

            // (Facoltativo) Pulizia dei campi della form
            RoutineNameTextBox.Clear();
            ProgramsListBox.UnselectAll();
        }
        // Evento per selezionare il percorso del programma tramite finestra di dialogo (per Add Program)
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ProgramPathTextBox.Text = openFileDialog.FileName;
            }
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
            ClearAddProgramData();

            // Aggiungi il ProgramItem a ProgramsManager e salva
            ProgramManager.AddProgram(newProgram);
            ProgramManager.Save();
            CreateButtons();

            // Torna alla vista principale
            InterfaceSelectorMode(0);
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e)
        {
            InterfaceSelectorMode(0);
            ClearAddProgramData();
        }

        private void ClearAddProgramData()
        {
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
        private void OnDeleteRoutineClicked(RoutineItem item)
        {
            MessageBoxResult result = MessageBox.Show($"Can I leave the case here? Are you sure you want to delete {item.Name}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Se l'utente conferma, chiama il metodo RemoveProgram del ProgramManager
                ProgramManager.RemoveRoutine(item);
                ProgramManager.Save();
            }
            CreateButtons();
        }

        private void InterfaceSelectorMode(int mode)
        {
            switch (mode)
            {
                case 0:
                    AddProgramPanel.Visibility = Visibility.Collapsed;
                    MainScrollViewer.Visibility = Visibility.Visible;
                    AddRoutinePanel.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    AddProgramPanel.Visibility = Visibility.Visible;
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                    AddRoutinePanel.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                    AddRoutinePanel.Visibility = Visibility.Visible;

                    break;
                default:
                    break;
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
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
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
            foreach (var item in ProgramManager.Routines)
            {
                // Crea il pulsante
                Button button = new Button
                {
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                };

                // Crea un DockPanel per inserire l'icona e il testo
                DockPanel dockPanel = new DockPanel();
                // Aggiungi l'icona al pulsante
                Image iconImage = new Image
                {
                    Source = LoadIcon("pack://application:,,,/list.png"),  // Ottieni l'icona
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 5, 0) // Margine tra icona e testo
                };
                DockPanel.SetDock(iconImage, Dock.Left);  // Posiziona l'icona a sinistra
                dockPanel.Children.Add(iconImage);
                // Aggiungi il nome del programma al pulsante
                TextBlock textBlock = new TextBlock
                {
                    Text = item.Name + "(Routine)",
                    VerticalAlignment = VerticalAlignment.Center
                };
                dockPanel.Children.Add(textBlock);
                button.Content = dockPanel;
                // Crea il ContextMenu con l'opzione "Delete Program"
                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem { Header = "Delete Program" };
                deleteMenuItem.Click += (s, e) => OnDeleteRoutineClicked(item); // Associa l'evento click
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