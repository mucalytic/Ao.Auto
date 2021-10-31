using System.Runtime.InteropServices;
using System;

namespace Ao.Auto.Ui
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
