using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Configuration;

namespace AdminLauncher.AppWPF
{
    public class HalfWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
                if (width > int.Parse(ConfigurationManager.AppSettings["WidthVertical"])-2)
                    return (width / 2) - 70;
                else
                    return width - 170;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
