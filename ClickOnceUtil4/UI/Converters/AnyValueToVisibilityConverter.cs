using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ClickOnceUtil4UI.UI.Converters
{
    /// <summary>
    /// Visible is property not null.
    /// </summary>
    public class AnyValueToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Invert result value.
        /// </summary>
        public bool Inverted { get; set; }

        /// <inheritdoc/>
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ^ Inverted ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <inheritdoc/>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
