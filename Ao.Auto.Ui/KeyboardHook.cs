using System.Runtime.InteropServices;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Diagnostics;
using System;

namespace Ao.Auto.Ui
{
    public class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
 
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private readonly ISubject<IntPtr>        _keySubject;
        private readonly LowLevelKeyboardProc _proc;
        private          IntPtr               _hookId = IntPtr.Zero;

        public IObservable<IntPtr> ObservableKeys =>
            _keySubject;

        public KeyboardHook()
        {
            _keySubject = new Subject<IntPtr>();
            _proc       = HookCallback;
        }
 
        public void HookKeyboard() =>
            _hookId = SetHook(_proc);
 
        public void UnHookKeyboard() =>
            User32.UnhookWindowsHookEx(_hookId);
 
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
                //_keySubject.OnNext(KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam)));
                _keySubject.OnNext(lParam);
            }
 
            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
}
