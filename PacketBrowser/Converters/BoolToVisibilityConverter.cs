using System;
using System.Windows;

namespace PacketBrowser.Converters
{
    public class BoolToVisibilityConverter : ConverterMarkupExtensionBase<BoolToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool && ((bool)value) == true)
                return Visibility.Visible;

            if (value is bool? && ((bool?)value).GetValueOrDefault(false) == true)
                return Visibility.Visible;

            if (parameter != null && parameter is Visibility)
                return parameter;

            return Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
