using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace Mistilteinn.Tools
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UnhandledExceptionWindow : Window
    {
        public UnhandledExceptionWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fd = new SaveFileDialog()
            {
                DefaultExt = "*.log",
                AddExtension = true,
                Filter = "Mis 异常报告(*.log)|*.log|所有文件(*.*)|*.*",
                Title = "保存 Mis 异常报告"
            };

            if ((bool)fd.ShowDialog(Application.Current.MainWindow))
            {
                File.WriteAllText(fd.FileName, Log.Text);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
