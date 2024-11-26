using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for ProgramsConfiguratorWizard.xaml
    /// </summary>
    public partial class ProgramsConfiguratorWizard : MetroWindow
    {
        public ObservableCollection<ProgramItemForListbox> ProgramList { get; set; }
        public ObservableCollection<ProgramItemForListbox> FilteredProgramList { get; set; }
        public List<ProgramItem> SelectedProgram { get; private set; }
        public ProgramsConfiguratorWizard(List<ProgramItem> currentProgramList, string theme)
        {
            InitializeComponent();

            double workAreaHeight = SystemParameters.WorkArea.Height;
            double workAreaWidth = SystemParameters.WorkArea.Width;

            if (workAreaWidth < this.Width)
                this.Width = workAreaWidth;
            if (workAreaHeight < this.Height)
                this.Height = workAreaHeight;

            ThemeManager.Current.ChangeTheme(this, theme);
            DataContext = this;

            // Retrieves the installed programs and creates the ProgramItemForListbox list
            var ProgramsInstalled = InstalledProgramUtility.GetInstalledProgram();
            ProgramList = new ObservableCollection<ProgramItemForListbox>(
                ProgramsInstalled.Select(p => new ProgramItemForListbox
                {
                    Program = p,
                    IsChecked = currentProgramList.Any(e => e.ExecutablePath == p.ExecutablePath && e.Arguments == p.Arguments)
                })
            );
            ProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
            FilteredProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
        }
        private void ProcessSelectedPrograms(object sender, RoutedEventArgs e)
        {
            // Filter selected programs
            SelectedProgram = ProgramList.Where(p => p.IsChecked).Select(p => p.Program).ToList();

            DialogResult = true;
            Close();
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = SearchBox.Text.ToLower();

            // Update filtered list
            FilteredProgramList.Clear();
            foreach (var program in ProgramList)
            {
                if (program.Program.Name.ToLower().Contains(filterText))
                {
                    FilteredProgramList.Add(program);
                }
            }
        }
    }
}
