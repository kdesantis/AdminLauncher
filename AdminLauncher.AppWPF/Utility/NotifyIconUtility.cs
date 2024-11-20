using AdminLauncher.BusinessLibrary;
using NLog;
using System.IO;
using System.Threading;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF.Utility
{
    public class NotifyIconUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private MainWindow window;
        private Manager manager;
        public NotifyIcon AppNotifyIcon { get; set; }
        public NotifyIconUtility(MainWindow mainWindow, Manager manager)
        {
            window = mainWindow;

            this.manager = manager;
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

            AddContextMenu(this.manager.programManager);

            AppNotifyIcon.DoubleClick += (s, e) => ShowWindow();
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
                {
                    Image image;
                    try
                    {
                        image = IconUtility.GetImageIcon(program.GetIconPath());
                    }
                    catch (Exception ex)
                    {
                        logger.Warn(ex, "CurrentProgram:{program.Index}.{program.Name}", program.Index, program.Name);
                        image = IconUtility.GetImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"));
                    }
                    contextMenu.Items.Add(program.Name, image, (sender, e) => program.Launch());
                }
            }
            if ((programManager.Routines.Count + programManager.Programs.Count) > 0)
                contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Quick Run", Properties.Resources.rocket, OnQuickRunClick);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Close", Properties.Resources.OnOff, OnCloseClick);
            AppNotifyIcon.ContextMenuStrip = contextMenu;
        }
        private void ShowWindow()
        {
            InterfaceControl.PositionWindowInBottomRight(window);
            window.Show();
            window.WindowState = WindowState.Normal;
        }
        private void OnCloseClick(object sender, EventArgs e)
        {
            window.firstClosure = false;
            Application.Current.Shutdown();
        }
        private void OnQuickRunClick(object sender, EventArgs e) =>
            QuickRunUtils.LaunchQuickRun(manager.settingsManager.InitialFileDialogPath, window.CurrentDialogUtility);
    }
}
