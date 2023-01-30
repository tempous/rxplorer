using LiteExplorer.Converters.Base;
using System;
using System.Globalization;
using System.Windows;

namespace LiteExplorer.Converters
{
    internal class ValueToVisibility : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double percent && percent == 100.0 ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
