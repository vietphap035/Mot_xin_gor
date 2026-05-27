using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Mot_xin_gor
{
    public class CheckToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isGroup = (bool)value;
            return isGroup ? "Nhập tên nhóm..." : "Nhập tên người dùng...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
