using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Mistilteinn.Texts;

namespace Mistilteinn.Converters
{
    public class TextInfomationToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof (String))
            {
                if (value != null)
                {
                    var info = (IList<TextFileInfomation>)value;
                    return $"{info[0][1]}\r\n{info[1][1]}";
                }
                else
                {
                    return null;
                }
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
