using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;
using System;
using Ao.Auto.Ui;

namespace Ao.Auto.Processes
{
    public class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
 
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
 
        public event EventHandler<KeyPressedArgs> OnKeyPressed;
 
        private readonly LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;
 
        public KeyboardHook()
        {
            _proc = HookCallback;
        }
 
        public void HookKeyboard()
        {
            _hookId = SetHook(_proc);
        }
 
        public void UnHookKeyboard()
        {
            User32.UnhookWindowsHookEx(_hookId);
        }
 
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return curModule is null ? IntPtr.Zero : User32.SetWindowsHookEx(WH_KEYBOARD_LL, proc, Kernel32.GetModuleHandle(curModule.ModuleName), 0);
        }
 
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);

                OnKeyPressed?.Invoke(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode)));
            }
 
            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
    
    public class KeyPressedArgs : EventArgs
    {
        public Key KeyPressed { get; private set; }
 
        public KeyPressedArgs(Key key)
        {
            KeyPressed = key;
        }
    }
}
