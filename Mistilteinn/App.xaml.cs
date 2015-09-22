using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace Mistilteinn
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ShowMainWindow();

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            File.AppendAllText("error.log", $"\r\n{JsonConvert.SerializeObject(e.Exception)}");
            MessageDialog.Show("应用中出现了异常，请查看error.log", "异常", Char.ConvertFromUtf32(0xe844));
            e.Handled = true;
            Application.Current.Shutdown();
        }

        public void ShowMainWindow()
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
