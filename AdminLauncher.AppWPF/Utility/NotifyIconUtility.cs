using System.IO;
using System.Windows;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF.Utility
{
    public static class NotifyIconUtility
    {
        /// <summary>
        /// Initialize the NotifyIcon of the application
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <returns></returns>
        public static NotifyIcon InitializeNotifyIcon(MainWindow mainWindow)
        {
            var icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"));
#if DEBUG
            icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocketDebug.ico"));
#endif
            NotifyIcon notifyIcon = new()
            {
                Icon = icon,
                Visible = true,
                Text = "Admin Launcher"
            };

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Close", null, OnCloseClick);
            notifyIcon.ContextMenuStrip = contextMenu;

            notifyIcon.DoubleClick += (s, e) => ShowWindow(mainWindow);
            return notifyIcon;
        }
        private static void ShowWindow(MainWindow mainWindow)
        {
            InterfaceControl.PositionWindowInBottomRight(mainWindow);
            mainWindow.Show();
            mainWindow.WindowState = WindowState.Normal;
        }
        private static void OnCloseClick(object sender, EventArgs e) =>
            Application.Current.Shutdown();


    }
}
