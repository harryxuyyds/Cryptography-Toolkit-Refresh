using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;

namespace Cryptography_Toolkit.Helpers
{
    internal static class VersionHelper
    {
        public static string WinAppSdkDetails =>
            $"Windows App SDK {ReleaseInfo.Major}.{ReleaseInfo.Minor}";

        public static string WinAppSdkRuntimeDetails =>
            WinAppSdkDetails + $", Windows App Runtime {RuntimeInfo.AsString}";
    }
}
