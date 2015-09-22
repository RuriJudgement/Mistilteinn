using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mistilteinn.ViewModels;

namespace Mistilteinn
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();

            this.Loaded += MessageWindow_Loaded;
        }

        private void MessageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void Yes_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MessageWindowViewModel).ResultButton = MessageDialogResultButton.Yes;
            Close();
        }

        private void No_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MessageWindowViewModel).ResultButton = MessageDialogResultButton.No;
            Close();
        }

        private void Ok_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MessageWindowViewModel).ResultButton = MessageDialogResultButton.Ok;
            Close();
        }

        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            (DataContext as MessageWindowViewModel).ResultButton = MessageDialogResultButton.Cancel;
            Close();
        }
    }
    
    public static class MessageDialog
    {
        public static MessageDialogResultButton Show(Window owner, String content, String titile, String icon, MessageDialogButtons buttons)
        {
            MessageWindowViewModel vm = new MessageWindowViewModel(titile, content, icon, buttons);
            var dialog = new MessageWindow()
            {
                DataContext = vm,
                Owner = owner
            };
            dialog.ShowDialog();
            return vm.ResultButton;
        }

        public static MessageDialogResultButton Show(Window owner, String content, String titile, String icon )
        {
            return Show(owner, content, titile, icon, MessageDialogButtons.Ok);
        }

        public static MessageDialogResultButton Show(Window owner, String content, String titile)
        {
            return Show(owner, content, titile, Char.ConvertFromUtf32(0xe84f));
        }

        public static MessageDialogResultButton Show(Window owner, String content)
        {
            return Show(owner, content, null);
        }

        public static MessageDialogResultButton Show(String content, String titile, String icon, MessageDialogButtons buttons)
        {
            return Show(Application.Current.MainWindow, content, titile, icon, buttons);
        }

        public static MessageDialogResultButton Show(String content, String titile, String icon)
        {
            return Show(Application.Current.MainWindow, content, titile, icon);
        }

        public static MessageDialogResultButton Show(String content, String titile)
        {
            return Show(Application.Current.MainWindow, content, titile);
        }

        public static MessageDialogResultButton Show(String content)
        {
            return Show(Application.Current.MainWindow, content);
        }
    }

    public enum MessageDialogButtons
    {
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel
    }

    public enum MessageDialogResultButton
    {
        Ok,
        Cancel,
        Yes,
        No
    }
}
