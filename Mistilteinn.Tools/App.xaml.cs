using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Mistilteinn.Tools.ViewModels;

namespace Mistilteinn.Tools
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        void RestartAsAdministraror(String args)
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                processInfo.Arguments = args;

                var process = Process.Start(processInfo);
                process.WaitForExit();
                Environment.Exit(process.ExitCode);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            switch (e.Args.Length)
            {
                case 1:
                    switch (e.Args[0])
                    {
                        case "RegistryExtendName":
                            if (SetFileExtend(String.Join(" ", e.Args)))
                            {
                                Shutdown((int)ExitCode.RegisterFileSuccess);
                            }
                            else
                            {
                                Shutdown((int)ExitCode.RegisterFileFail);
                            }
                            break;
                        default:
                            Shutdown((int)ExitCode.None);
                            break;
                    }
                    break;
                case 2:
                    switch (e.Args[0])
                    {
                        case "ReportException":
                            new UnhandledExceptionWindow()
                            {
                                DataContext = new UnhandledExceptionWindowViewModel(e.Args[1])
                            }.Show();
                            break;
                        default:
                            Shutdown((int)ExitCode.None);
                            break;
                    }
                    break;
                default:
                    Shutdown((int)ExitCode.None);
                    break;
            }
        }
        private bool SetFileExtend(String args)
        {
            try
            {
                string appPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Mistilteinn.exe");
                string extendName = ".mis";
                string extendDefaultSet = "MIS";
                string directions = "Mistilteinn 翻译工程文件";

                var registry = Registry.ClassesRoot.OpenSubKey(extendDefaultSet)?.OpenSubKey("shell")?.OpenSubKey("open")?.OpenSubKey("command");
                var value = registry?.GetValue("").ToString();
                var c = $"{appPath} \"%1\"";
                if (registry != null && value == c)
                {
                    return false;
                }
                
                RestartAsAdministraror(args);

                if (registry != null)
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(extendDefaultSet);
                }
                RegistryKey key;

                key = Registry.ClassesRoot.CreateSubKey(extendDefaultSet);
                key.SetValue("", directions);
                key = key.CreateSubKey("shell");
                key = key.CreateSubKey("open");
                key = key.CreateSubKey("command");
                key.SetValue("", $"{appPath} \"%1\"");

                key = Registry.ClassesRoot.OpenSubKey(extendDefaultSet, true);
                key = key.CreateSubKey("DefaultIcon");
                key.SetValue("", $"{appPath},0");

                key = Registry.ClassesRoot.CreateSubKey(extendName);
                key.SetValue("", extendDefaultSet);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    public enum ExitCode
    {
        None = 0x0,
        RegisterFileSuccess = 0x100,
        RegisterFileFail = 0x101,
    }
}
