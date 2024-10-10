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
    public class ButtonGenerator
    {
        protected ProgramManager? Manager { get; set; }
        protected MainWindow? Window { get; set; }

        /// <summary>
        /// Populates the StackPanel “ButtonPanel” of the MainWindows with all the programs and routines in the program manager
        /// </summary>
        /// <param name="programManager"></param>
        /// <param name="mainWindow"></param>
        public void GenerateButtons(ProgramManager programManager, MainWindow mainWindow)
        {
            Manager = programManager;
            Window = mainWindow;

            new VerticalButtonsGenerator().GenerateVerticalButtons(programManager, mainWindow);
            //new HorizontalButtonsGenerator().GenerateHorizontalButtons(Manager, Window);
        }
        protected List<GenericItem> GetSortedGenericItems()
        {
            return
            [
                .. Manager.Routines.OrderBy(e => e.Name),
                .. Manager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite),
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

        protected void OnDeleteClicked(GenericItem item)
        {
            if (ConfirmDeletion(item.Name))
            {
                RemoveItem(item);
                Manager.Save();
                GenerateButtons(Manager, Window);
            }
        }

        private static bool ConfirmDeletion(string itemName)
        {
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete {itemName}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            return result == MessageBoxResult.Yes;
        }

        protected void RemoveItem(GenericItem item)
        {
            if (item is ProgramItem program)
                Manager.RemoveProgram(program);
            else if (item is RoutineItem routine)
                Manager.RemoveRoutine(routine);
        }
        protected void OnEditClicked(GenericItem item)
        {
            if (item is ProgramItem programItem)
                EditProgram(programItem);
            else if (item is RoutineItem routineItem)
                EditRoutine(routineItem);
        }

        protected void EditProgram(ProgramItem program)
        {
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddProgramInterface, Window);
            Window.ProgramIndexLabel.Content = program.Index;
            Window.ProgramNameTextBox.Text = program.Name;
            Window.ProgramPathTextBox.Text = program.ExecutablePath;
            Window.ProgramArgumentsTextBox.Text = program.Arguments;
            Window.FavoriteCheckBox.IsChecked = program.IsFavorite;
        }

        protected void EditRoutine(RoutineItem routine)
        {
            InterfaceControl.LoadProgramsListBox(Manager.Programs, Window, routine);
            InterfaceControl.InterfaceLoader(InterfaceEnum.AddRoutineInterface, Window);
            Window.RoutineIndexLabel.Content = routine.Index;
            Window.RoutineNameTextBox.Text = routine.Name;
        }
    }
}
