using AdminLauncher.BusinessLibrary;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
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
            GenerateButtons();
            InitializeNotifyIcon();
        }

        private void PositionWindowInBottomRight()
        {
            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaWidth = SystemParameters.WorkArea.Width;

            this.Left = workAreaWidth - this.Width;
            this.Top = workAreaHeight - this.Height;
        }
        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico")),
                Visible = true,
                Text = "Admin Launcher"
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Close", null, OnCloseClick);
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => ShowWindow();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ShowWindow()
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void OnCloseClick(object sender, EventArgs e) =>
            Application.Current.Shutdown();

        private void Window_Closed(object sender, EventArgs e) =>
            notifyIcon.Dispose();
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void AddProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceLoader(InterfaceEnum.AddProgramInterface);

        private void AddRoutine_Click(object sender, RoutedEventArgs e)
        {
            InterfaceLoader(InterfaceEnum.AddRoutineInterface);

            ProgramsListBox.Items.Clear();
            foreach (var program in ProgramManager.Programs)
                ProgramsListBox.Items.Add(program.Name);
        }
        private void SaveRoutine_Click(object sender, RoutedEventArgs e)
        {
            RoutineItem newRoutine = new RoutineItem
            {
                Name = RoutineNameTextBox.Text,
                Programs = new List<ProgramItem>()
            };

            foreach (var selectedProgram in ProgramsListBox.SelectedItems)
            {
                var program = ProgramManager.FindProgramByName(selectedProgram.ToString());
                if (program != null)
                {
                    newRoutine.AddProgram(program);
                }
            }

            ProgramManager.AddRoutine(newRoutine);
            ProgramManager.Save();

            InterfaceLoader(InterfaceEnum.MainInterface);
            GenerateButtons();
        }
        private void CancelRoutine_Click(object sender, RoutedEventArgs e) =>
            InterfaceLoader(InterfaceEnum.MainInterface);
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                ProgramPathTextBox.Text = openFileDialog.FileName;
        }
        private void SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            ProgramItem newProgram = new ProgramItem
            {
                Name = ProgramNameTextBox.Text,
                Path = ProgramPathTextBox.Text,
                Arguments = ProgramArgumentsTextBox.Text,
                IsFavorite = FavoriteCheckBox.IsChecked == true
            };

            ProgramManager.AddProgram(newProgram);
            ProgramManager.Save();
            GenerateButtons();

            InterfaceLoader(InterfaceEnum.MainInterface);
        }
        private void CancelProgram_Click(object sender, RoutedEventArgs e) =>
            InterfaceLoader(InterfaceEnum.MainInterface);
        private void ClearAddProgramData()
        {
            ProgramNameTextBox.Clear();
            ProgramPathTextBox.Clear();
            ProgramArgumentsTextBox.Clear();
            FavoriteCheckBox.IsChecked = false;
        }
        private void ClearRoutineData()
        {
            RoutineNameTextBox.Clear();
            ProgramsListBox.UnselectAll();
        }
        private void OnDeleteClicked(GenericItem item)
        {
            MessageBoxResult result = MessageBox.Show($"Can I leave the case here? Are you sure you want to delete {item.Name}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (item is ProgramItem)
                    ProgramManager.RemoveProgram((ProgramItem)item);
                else
                    ProgramManager.RemoveRoutine((RoutineItem)item);
                ProgramManager.Save();
            }
            GenerateButtons();
        }
        private void InterfaceLoader(InterfaceEnum mode)
        {
            switch (mode)
            {
                case InterfaceEnum.MainInterface:
                    AddProgramPanel.Visibility = Visibility.Collapsed;
                    MainScrollViewer.Visibility = Visibility.Visible;
                    AddRoutinePanel.Visibility = Visibility.Collapsed;
                    ClearAddProgramData();
                    ClearRoutineData();
                    break;
                case InterfaceEnum.AddProgramInterface:
                    AddProgramPanel.Visibility = Visibility.Visible;
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                    AddRoutinePanel.Visibility = Visibility.Collapsed;
                    break;
                case InterfaceEnum.AddRoutineInterface:
                    MainScrollViewer.Visibility = Visibility.Collapsed;
                    AddProgramPanel.Visibility = Visibility.Collapsed;
                    AddRoutinePanel.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void GenerateButtons()
        {
            ButtonPanel.Children.Clear();

            List<GenericItem> genericItems = new List<GenericItem>();
            genericItems.AddRange(ProgramManager.Routines);
            genericItems.AddRange(ProgramManager.Programs);

            foreach (var item in genericItems)
            {
                Button button = new Button
                {
                    Margin = new Thickness(5),
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                };

                DockPanel dockPanel = new DockPanel();
                Image iconImage = new Image
                {
                    Source = LoadIcon(item.GetIconPath()),
                    Width = 32,
                    Height = 32,
                    Margin = new Thickness(0, 0, 5, 0)
                };
                DockPanel.SetDock(iconImage, Dock.Left);
                dockPanel.Children.Add(iconImage);

                TextBlock textBlock = new TextBlock
                {
                    Text = item.Name,
                    VerticalAlignment = VerticalAlignment.Center
                };
                dockPanel.Children.Add(textBlock);
                button.Content = dockPanel;

                ContextMenu contextMenu = new ContextMenu();
                MenuItem deleteMenuItem = new MenuItem { Header = item is ProgramItem ? "Delete Program" : "Delete Routine" };
                deleteMenuItem.Click += (s, e) => OnDeleteClicked(item);
                contextMenu.Items.Add(deleteMenuItem);
                button.ContextMenu = contextMenu;

                button.Click += (sender, e) => item.Launch();

                ButtonPanel.Children.Add(button);
            }
        }
        private static BitmapImage LoadIcon(string iconPath)
        {
            BitmapImage bitmap = new BitmapImage();
            if (iconPath != null)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }
    }
}