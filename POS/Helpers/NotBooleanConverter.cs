using System;
using System.Globalization;
using System.Windows.Data;

namespace POS.Helpers
{
    /// <summary>
    /// Convertidor que invierte un valor booleano.
    /// Utilizado en XAML para deshabilitar botones mientras se carga.
    /// </summary>
    public class NotBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }
}
