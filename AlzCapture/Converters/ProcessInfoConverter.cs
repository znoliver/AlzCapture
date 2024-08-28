using System;
using System.Diagnostics;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AlzCapture.Converters;

public class ProcessInfoConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Process process)
        {
            try
            {
                return process.MainModule?.FileName ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}