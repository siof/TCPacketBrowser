using siof.Common.Extensions;
using System;

namespace siof.Common.Wpf.Converters
{
    public class StringSingleLineConverter : ConverterMarkupExtensionBase<StringSingleLineConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToStringSafe().ReplaceSafe(Environment.NewLine, " ");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
