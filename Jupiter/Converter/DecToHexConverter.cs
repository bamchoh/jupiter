using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Jupiter.Converter
{
    public class FormatType
    {
        public const string DEC = "Decimal";
        public const string HEX = "Hexadecimal";
        public const string OCT = "Octal";
        public const string BOOL = "Boolean";
        public const string FLOAT = "Float";
        public const string STRING = "String";
        public const string DATE_AND_TIME = "Date/Time";
        public const string VARIANT = "Variant";
        public const string NULL = "Null";
    }

    /*
    public class AnyToBoolConverter : IValueConverter
    {
        IWrappedValueConverter conv;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            conv = value as IWrappedValueConverter;

            if (conv != null)
            {
                try
                {
                    var convValue = conv.ConvertValue();
                    if (convValue is bool)
                    {
                        return convValue;
                    }
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                return conv.ConvertValueBack(value);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }

    public class DecToHexConverter : IValueConverter
    {
        IWrappedValueConverter conv;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            conv = value as IWrappedValueConverter;

            if (conv != null)
                return conv.ConvertValue();

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            try
            {
                return conv.ConvertValueBack(value);
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
    */
}
