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
    public class TextInfomationsToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(String))
            {
                if (value != null)
                {
                    var info = (IList<TextFileInfomation>)value;
                    return String.Join("\r\n", info.Select(s => s.ToString()));
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
            if (value != null)
            {
                IList<TextFileInfomation> infos = Activator.CreateInstance(targetType) as IList<TextFileInfomation>;
                foreach (var s in value.ToString().Replace("\r\n", "\n").Split('\n'))
                {
                    if (s.EndsWith("]") && s.StartsWith("*["))
                    {
                        infos.Add(new TextFileInfomation(s.Substring(2, s.Length - 3).Split(',')));
                    }
                }
                return infos;
            }
            else
            {
                return null;
            }
        }
    }
}
