using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbeXP.Helpers
{
    /// <summary>
    /// Convert a boolean value to a Visibility enumeration.
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool flag)
                return Visibility.Visible; // default visible if wrong type

            // When true → visible
            if (flag)
                return Visibility.Visible;

            // When false → check parameter
            var mode = parameter?.ToString()?.ToLowerInvariant();

            return mode switch
            {
                "collapse" => Visibility.Collapsed,
                "hidden" or null or "" => Visibility.Hidden,
                _ => Visibility.Hidden
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
