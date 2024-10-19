using AdminLauncher.BusinessLibrary;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;

namespace AdminLauncher.AppWPF.Utility
{
    public static class IconUtility
    {
        /// <summary>
        /// Restores the BitMapImage version of the icon to the indexed path
        /// </summary>
        /// <param name="iconPath"></param>
        /// <returns></returns>
        public static BitmapImage LoadIcon(string iconPath)
        {
            BitmapImage bitmap = new();
            if (iconPath != null)
            {
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
            }
            return bitmap;
        }

        public static void DeleteTempIcon()
        {
            var iconDirectoryPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), ConfigurationManager.AppSettings["IconTempDirectoryName"]);
            if (Directory.Exists(iconDirectoryPath))
                Directory.Delete(iconDirectoryPath, true);
        }

    }
}
