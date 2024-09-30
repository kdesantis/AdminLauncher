using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF.Utility
{
    public static class NotifyIconUtility
    {
        public static NotifyIcon InitializeNotifyIcon(MainWindow mainWindow)
        {
            NotifyIcon notifyIcon = new NotifyIcon
            {
                Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico")),
                Visible = true,
                Text = "Admin Launcher"
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Close", null, OnCloseClick);
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => ShowWindow(mainWindow);
            return notifyIcon;
        }
        public static void ShowWindow(MainWindow mainWindow)
        {
            InterfaceControl.PositionWindowInBottomRight(mainWindow);
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }
        private static void OnCloseClick(object sender, EventArgs e) =>
            Application.Current.Shutdown();


    }
}
