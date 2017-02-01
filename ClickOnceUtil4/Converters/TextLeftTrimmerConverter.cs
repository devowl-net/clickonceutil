using System;
using System.Globalization;
using System.Windows.Data;

namespace ClickOnceUtil4UI.Converters
{
    /// <summary>
    /// Trim from left text converter.
    /// </summary>
    public class TextLeftTrimmerConverter : IValueConverter
    {
        private const string DefaultPrefix = "...";

        /// <summary>
        /// Max text length.
        /// </summary>
        public int MaxLength { get; set; } = int.MaxValue;

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var text = value.ToString();
            if (text.Length > MaxLength)
            {
                return $"{DefaultPrefix}{text.Substring(text.Length - MaxLength, MaxLength)}";
            }

            return text;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}