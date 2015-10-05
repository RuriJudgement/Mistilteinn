using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Hosting;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Mistilteinn
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Project.Current?.Save("Backup.mis");
            Helper.Tools.ReportException(e.Exception as Exception);
            e.Handled = true;
            Shutdown();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Project.Current?.Save("Backup.mis");
            Helper.Tools.ReportException(e.ExceptionObject as Exception);
            Shutdown();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (e.Args.Length == 1)
            {
                if (File.Exists(e.Args[0]))
                {
                    Project.LoadProject(e.Args[0]);
                    if (Project.Current == null)
                    {
                        Shutdown();
                    }
                }
                else
                {
                    Shutdown();
                }
            }
            else if(e.Args.Length != 0)
            {
                if (MessageDialog.Show("错误的运行参数，是否正常打开 Mistilteinn？", "参数错误", Char.ConvertFromUtf32(0xe886),
                        MessageDialogButtons.YesNo) == MessageDialogResultButton.No)
                {
                    Shutdown();
                }
            }

            Helper.Tools.RegisterFile();

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
