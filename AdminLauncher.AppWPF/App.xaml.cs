using NLog;
using Application = System.Windows.Application;

namespace AdminLauncher.AppWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public App()
        {
            // Handles unhandled exceptions in the main UI thread
            this.DispatcherUnhandledException += OnDispatcherUnhandledException;

            // Handles unhandled exceptions in other threads
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Unhandled exception in main thread");
            //e.Handled = true; 
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                Logger.Fatal(exception, "Fatal exception not handled");
            }
            else
            {
                Logger.Fatal("Unmanaged exception: " + e.ExceptionObject);
            }
        }
    }

}
