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
using Opc.Ua;
using System.Drawing;
using System.Windows.Media;

namespace Jupiter.Converter
{
    public class StatusCodeColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (ServiceResult.IsNotGood((StatusCode)value))
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#DB585C"));

            return new SolidColorBrush(System.Windows.Media.Colors.Black);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
