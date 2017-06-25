using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace siof.Common.Wpf.Converters
{
    public abstract class ConverterMarkupExtensionBase<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        private static T m_converter = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (m_converter == null)
            {
                m_converter = new T();
            }
            return m_converter;
        }

        #region IValueConverter Members

        public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);

        #endregion IValueConverter Members
    }
}
