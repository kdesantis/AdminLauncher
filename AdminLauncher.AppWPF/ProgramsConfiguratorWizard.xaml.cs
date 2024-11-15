using AdminLauncher.AppWPF.Utility;
using AdminLauncher.BusinessLibrary;
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
    public partial class ProgramsConfiguratorWizard : Window
    {
        public ObservableCollection<ProgramItemForListbox> ProgramList { get; set; }
        public ObservableCollection<ProgramItemForListbox> FilteredProgramList { get; set; }
        public ProgramsConfiguratorWizard()
        {
            InitializeComponent();
            DataContext = this;

            // Recupera i programmi installati e crea la lista di ProgramItemForListbox
            var ProgramsInstalled = Utility.InstalledProgramUtility.GetInstalledProgram();
            ProgramList = new ObservableCollection<ProgramItemForListbox>(
                ProgramsInstalled.Select(p => new ProgramItemForListbox
                {
                    Program = p,
                    IsChecked = false
                })
            );
            ProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
            FilteredProgramList = new ObservableCollection<ProgramItemForListbox>(ProgramList);
        }
        private void ProcessSelectedPrograms(object sender, RoutedEventArgs e)
        {
            // Filtra i programmi selezionati
            var selectedPrograms = FilteredProgramList.Where(p => p.IsChecked).Select(p => p.Program).ToList();

            // Esegui la logica con la lista selezionata
            foreach (var program in selectedPrograms)
            {
                int i = 0;
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = SearchBox.Text.ToLower();

            // Aggiorna la lista filtrata
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
