﻿using AdminLauncher.BusinessLibrary;
using System.IO;
using System.Threading;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF.Utility
{
    public class NotifyIconUtility
    {
        private MainWindow window;
        public NotifyIcon AppNotifyIcon { get; set; }
        public NotifyIconUtility(MainWindow mainWindow, ProgramManager programManager)
        {
            window = mainWindow;

            window = mainWindow;
            var icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"));
#if DEBUG
            icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocketDebug.ico"));
#endif
            AppNotifyIcon = new()
            {
                Icon = icon,
                Visible = true,
                Text = "Admin Launcher"
            };

            AddContextMenu(programManager);

            AppNotifyIcon.DoubleClick += (s, e) => ShowWindow(mainWindow);
        }

        public void AddContextMenu(ProgramManager programManager)
        {
            var contextMenu = new System.Windows.Forms.ContextMenuStrip();

            if (programManager != null)
            {
                foreach (var routine in programManager.Routines.OrderBy(e => e.Name).ToList())
                    contextMenu.Items.Add(routine.Name, Image.FromFile(routine.GetIconPath()), (sender, e) => routine.Launch());
                if (programManager.Routines.Count > 0)
                    contextMenu.Items.Add(new ToolStripSeparator());

                foreach (var program in programManager.Programs.OrderBy(e => e.Name).OrderByDescending(e => e.IsFavorite).ToList())
                    contextMenu.Items.Add(program.Name, Image.FromFile(program.GetIconPath()), (sender, e) => program.Launch());
            }
            if ((programManager.Routines.Count + programManager.Programs.Count) > 0)
                contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Quick Run", Properties.Resources.rocket, OnQuickRunClick);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Close", Properties.Resources.OnOff, OnCloseClick);
            AppNotifyIcon.ContextMenuStrip = contextMenu;
        }
        private void ShowWindow(MainWindow mainWindow)
        {
            InterfaceControl.PositionWindowInBottomRight(mainWindow);
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }
        private void OnCloseClick(object sender, EventArgs e)
        {
            window.firstClosure = false;
            Application.Current.Shutdown();
        }
        private void OnQuickRunClick(object sender, EventArgs e) =>
            QuickRunUtils.LaunchQuickRun();
    }
}
