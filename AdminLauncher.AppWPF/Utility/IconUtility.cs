using AdminLauncher.AppWPF.Properties;
using AdminLauncher.BusinessLibrary;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;

namespace AdminLauncher.AppWPF.Utility
{
    public static class IconUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Restores the BitMapImage version of the icon to the indexed path
        /// </summary>
        /// <param name="iconPath"></param>
        /// <returns></returns>
        public static BitmapImage GetBitmapImageIcon(string iconPath)
        {
            logger.Info($"start GetBitmapImageIcon");
            try
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
            catch (Exception ex)
            {
                logger.Warn(ex,"path{iconPath}",iconPath);
                return GetBitmapImageIcon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rocket.ico"));
            }
            return null;
        }

        public static Image GetImageIcon(string iconPath)
        {
            // test image
            BitmapImage image = new BitmapImage(new Uri(iconPath));

            // copy to byte array
            int stride = image.PixelWidth * 4;
            byte[] buffer = new byte[stride * image.PixelHeight];
            image.CopyPixels(buffer, stride, 0);

            // create bitmap
            System.Drawing.Bitmap bitmap =
                new System.Drawing.Bitmap(
                    image.PixelWidth,
                    image.PixelHeight,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // lock bitmap data
            System.Drawing.Imaging.BitmapData bitmapData =
                bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    bitmap.PixelFormat);

            // copy byte array to bitmap data
            System.Runtime.InteropServices.Marshal.Copy(
                buffer, 0, bitmapData.Scan0, buffer.Length);

            // unlock
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
