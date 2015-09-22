using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mistilteinn.Commands
{
    public static class MessageDialogButtonCommands
    {
        public static readonly RoutedUICommand OkCommand = new RoutedUICommand("确定", "OkCommand", typeof(MessageDialogButtonCommands));
        public static readonly RoutedUICommand CancelCommand = new RoutedUICommand("取消", "CancelCommand", typeof(MessageDialogButtonCommands));
        public static readonly RoutedUICommand YesCommand = new RoutedUICommand("是", "OkCommand", typeof(MessageDialogButtonCommands));
        public static readonly RoutedUICommand NoCommand = new RoutedUICommand("否", "OkCommand", typeof(MessageDialogButtonCommands));
    }
}
