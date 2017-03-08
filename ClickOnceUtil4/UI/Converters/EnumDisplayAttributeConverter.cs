using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

using ClickOnceUtil4UI.Utils;

namespace ClickOnceUtil4UI.UI.Converters
{
    /// <summary>
    /// Extract <see cref="DescriptionAttribute"/> to formatted string.
    /// </summary>
    public class EnumDisplayAttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = value as Enum;
            if (enumValue == null)
            {
                return value;
            }

            return $"{value}   ({enumValue.GetDescription()})";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}