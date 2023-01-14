using LiteExplorer.Converters.Base;
using System;
using System.Globalization;
using System.Windows.Media;

namespace LiteExplorer.Converters
{
    internal class PercentToColor : Converter
    {
        private readonly Color dangerColor = Color.FromRgb(150, 50, 0);
        private readonly Color warningColor = Color.FromRgb(150, 150, 50);
        private readonly Color infoColor = Color.FromRgb(50, 150, 50);

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percent = System.Convert.ToDouble(value);
            if (percent > 90)
                return new SolidColorBrush(dangerColor);
            else if (percent > 70)
                return new SolidColorBrush(warningColor);
            else
                return new SolidColorBrush(infoColor);
        }
    }
}
