using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mistilteinn.Controls
{
    public class MessageDialogButton : Button
    {
        public MessageDialogButton()
        {
            
        }

        public MessageDialogButton( String content, Brush foregroundBrush = null, Style style = null)
        {
            Content = content;
            if (foregroundBrush != null)
            {
                Foreground = foregroundBrush;
            }
            if (style != null)
            {
                Style = style;
            }
        }

        static MessageDialogButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialogButton), new FrameworkPropertyMetadata(typeof(MessageDialogButton)));
        }
    }
}
