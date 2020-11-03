using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Jupiter.Converter
{
    class BoolToContentConverter : IValueConverter
    {
        public string TrueConetnt { get; set; }

        public string FalseContent { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueConetnt;
            else
                return FalseContent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}