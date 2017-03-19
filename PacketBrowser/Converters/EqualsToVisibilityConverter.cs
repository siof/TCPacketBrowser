using System;
using System.Windows;

namespace PacketBrowser.Converters
{
    public class EqualsToVisibilityConverter : ConverterMarkupExtensionBase<EqualsToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null && parameter == null)
                return Visibility.Visible;

            if (value.Equals(parameter))
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
