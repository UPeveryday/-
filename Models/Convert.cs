using System;
using System.Globalization;
using System.Windows.Data;

namespace PortableEquipment.Models
{
    public class Convert : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value / 1.4;

        }
    }
}
