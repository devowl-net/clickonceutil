using System;
using System.Windows.Data;

namespace ClickOnceUtil4UI.Converters
{
    /// <summary>
    /// String to <see cref="Uri"/> converter.
    /// </summary>
    public class StringToUriConverter : IValueConverter
    {
        /// <inheritdoc/>
        object IValueConverter.Convert(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            var input = value as string;
            return string.IsNullOrEmpty(input) ? null : new Uri(input, UriKind.Absolute);
        }

        /// <inheritdoc/>
        object IValueConverter.ConvertBack(
            object value,
            Type targetType,
            object parameter,
            System.Globalization.CultureInfo culture)
        {
            var input = value as Uri;
            return input?.ToString() ?? string.Empty;
        }
    }
}