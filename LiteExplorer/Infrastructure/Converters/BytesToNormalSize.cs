using LiteExplorer.Infrastructure.Converters.Base;
using System;
using System.Globalization;

namespace LiteExplorer.Infrastructure.Converters;

internal class BytesToNormalSize : Converter
{
    private const double BytesInKB = 1024;
    private const double BytesInMB = 1048576;
    private const double BytesInGB = 1073741824;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var spaceInBytes = System.Convert.ToDouble(value);
        if (spaceInBytes + 4096 >= BytesInGB)
            return $"{spaceInBytes / BytesInGB:0.00} GB";
        else if (spaceInBytes >= BytesInMB)
            return $"{spaceInBytes / BytesInMB:0.00} MB";
        else if (spaceInBytes >= BytesInKB)
            return $"{spaceInBytes / BytesInKB:0.00} KB";
        else
            return $"{spaceInBytes:0.00} B";
    }
}
