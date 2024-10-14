using System.IO;
using System.Threading;
using System.Windows;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF.Utility
{
    public class NotifyIconUtility
    {
        private MainWindow window;
        public NotifyIcon AppNotifyIcon { get; set; }
        public NotifyIconUtility(MainWindow mainWindow)
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

            var contextMenu = new System.Windows.Forms.ContextMenuStrip();
            contextMenu.Items.Add("Close", null, OnCloseClick);
            AppNotifyIcon.ContextMenuStrip = contextMenu;

            AppNotifyIcon.DoubleClick += (s, e) => ShowWindow(mainWindow);
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
    }
}
