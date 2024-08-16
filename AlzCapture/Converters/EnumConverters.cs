using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AlzCapture.Converters;

public class EnumToIntConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Enum)
        {
            return (int)value;
        }

        return -1;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}