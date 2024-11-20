using AdminLauncher.BusinessLibrary;
using System.Windows.Controls;

namespace AdminLauncher.AppWPF.Utility
{
    public class ButtonsGenerator
    {
        protected Manager? Manager { get; set; }
        protected MainWindow? Window { get; set; }

        public ButtonsGenerator(Manager manager, MainWindow mainWindow)
        {
            this.Manager = manager;
            this.Window = mainWindow;
        }

        /// <summary>
        /// Populates the StackPanel “ButtonPanel” of the MainWindows with all the programs and routines in the program manager
        /// </summary>
        /// <param name="programManager"></param>
        /// <param name="mainWindow"></param>
        public void GenerateButtons()
        {
            if (Manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Mosaic)
            {
                new MosaicButtonsGenerator(Manager, Window).GenerateHorizontalButtons();
            }
            else if (Manager.settingsManager.ButtonsOrientation == OrientationsButtonEnum.Vertical)
            {
                new VerticalButtonsGenerator(Manager, Window).GenerateVerticalButtons();
            }

        }
        protected List<GenericItem> GetSortedGenericItems()
        {
            return
            [
                .. Manager.programManager.Routines.OrderBy(e => e.Name),
                .. Manager.programManager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite),
            ];
        }

        protected ContextMenu CreateContextMenu(GenericItem item)
        {
            ContextMenu contextMenu = new();

            MenuItem editMenuItem = new() { Header = item is ProgramItem ? "Edit Program" : "Edit Routine" };
            editMenuItem.Click += (s, e) => OnEditClicked(item);
            contextMenu.Items.Add(editMenuItem);

            MenuItem deleteMenuItem = new() { Header = item is ProgramItem ? "Delete Program" : "Delete Routine" };
            deleteMenuItem.Click += (s, e) => OnDeleteClicked(item);
            contextMenu.Items.Add(deleteMenuItem);

            return contextMenu;
        }

        protected async void OnDeleteClicked(GenericItem item)
        {
            if (await Window.CurrentDialogUtility.ConfirmDeletionAsync(item.Name))
            {
                RemoveItem(item);
                Manager.Save();
                Window.ReloadPrograms();
            }
        }

        private void RemoveItem(GenericItem item)
        {
            if (item is ProgramItem program)
                Manager.programManager.RemoveProgram(program);
            else if (item is RoutineItem routine)
                Manager.programManager.RemoveRoutine(routine);
        }
        private void OnEditClicked(GenericItem item)
        {
            if (item is ProgramItem programItem)
                EditProgram(programItem);
            else if (item is RoutineItem routineItem)
                EditRoutine(routineItem);
        }

        private void EditProgram(ProgramItem program)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, Window);
            Window.ProgramIndexLabel.Content = program.Index;
            Window.ProgramNameTextBox.Text = program.Name;
            Window.ProgramPathTextBox.Text = program.ExecutablePath;
            Window.ProgramArgumentsTextBox.Text = program.Arguments;
            Window.FavoriteCheckBox.IsChecked = program.IsFavorite;
        }

        private void EditRoutine(RoutineItem routine)
        {
            Window.UIOperation = false;
            InterfaceControl.LoadProgramsListBox(Manager.programManager.Programs, Window, routine);
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, Window);
            Window.RoutineIndexLabel.Content = routine.Index;
            Window.RoutineNameTextBox.Text = routine.Name;
        }
    }

}
