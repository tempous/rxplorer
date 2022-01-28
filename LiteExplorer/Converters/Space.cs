using LiteExplorer.Converters.Base;
using System;
using System.Globalization;

namespace LiteExplorer.Converters
{
    class Space : Converter
    {
        const double bytesInKB = 1024;
        const double bytesInMB = 1048576;
        const double bytesInGB = 1073741824;

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var spaceInBytes = System.Convert.ToDouble(value) + 4096;
            if (spaceInBytes >= bytesInGB)
                return $"{spaceInBytes / bytesInGB:0.00} GB";
            else if (spaceInBytes >= bytesInMB)
                return $"{spaceInBytes / bytesInMB:0.00} MB";
            else if (spaceInBytes >= bytesInKB)
                return $"{spaceInBytes / bytesInKB:0.00} KB";
            else
                return $"{spaceInBytes:0.00} B";
        }
    }
}
