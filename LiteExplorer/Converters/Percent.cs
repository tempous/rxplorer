using LiteExplorer.Converters.Base;
using System;
using System.Globalization;

namespace LiteExplorer.Converters
{
    internal class Percent : MultiConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var total = System.Convert.ToDouble(values[0]);
            var used = System.Convert.ToDouble(values[1]);
            return used / total * 100;
        }
    }
}
