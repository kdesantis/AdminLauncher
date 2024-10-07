using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

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

    }
}
