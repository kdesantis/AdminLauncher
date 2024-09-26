using AdminLauncher.BusinessLibrary;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Toolbelt.Drawing;

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
                    Source = LoadIconFromEXE(item),  // Ottieni l'icona
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

                // Assegna il DockPanel come contenuto del pulsante
                button.Content = dockPanel;

                // Associa il click del pulsante al metodo specifico dell'oggetto
                button.Click += (sender, e) => item.Launch();

                // Aggiungi il pulsante al pannello
                ButtonPanel.Children.Add(button);
            }
        }
        //https://www.codeproject.com/Articles/26824/Extract-icons-from-EXE-or-DLL-files
        //potrebbe essere utile
        private BitmapImage LoadIconFromEXE(ProgramItem programItem)
        {
            var iconPath= System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"{programItem.Index}-{programItem.Name}.ico");
            var s = File.Create(iconPath);
            IconExtractor.Extract1stIconTo(programItem.Path, s);
            s.Close();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(iconPath, UriKind.Absolute); // Usa UriKind.Absolute se il percorso è assoluto
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Cache per evitare blocchi di file
            bitmap.EndInit();
            return bitmap;
        }
    }
}