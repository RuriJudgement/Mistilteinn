using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mistilteinn.Helper
{
    public static class Tools
    {
        public async static Task<bool> RegisterFile()
        {
            var processInfo = new ProcessStartInfo("Mistilteinn.Tools.exe")
            {
                Arguments = "RegistryExtendName"
            };
            var process = new Process {StartInfo = processInfo};
            process.Start();

            await Task.Run(() =>
            {
                process.WaitForExit();
            });

            return process.ExitCode == (int)ExitCode.RegisterFileSuccess;
        }

        public static void ReportException(Exception exception)
        {
            var processInfo = new ProcessStartInfo("Mistilteinn.Tools.exe")
            {
                Arguments = $"ReportException {Uri.EscapeDataString(exception.ToString())}"
            };
            var process = new Process { StartInfo = processInfo };

            process.Start();
        }
    }

    public enum ExitCode
    {
        None = 0x0,
        RegisterFileSuccess = 0x100,
        RegisterFileFail = 0x101,
    }
}
