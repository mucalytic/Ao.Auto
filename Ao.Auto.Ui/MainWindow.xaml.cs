using System.Reactive.Disposables;
using System.Windows.Threading;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;

namespace Ao.Auto.Ui
{
    public partial class MainWindow : Window
    {
        private const string GameName = "Anarchy Online";
        private const uint WM_CHAR = 0x0102;
        
        private KeyboardHook                         _keyboardHook;
        private IDisposable                          _subscription;
        private IConnectableObservable<KeyProcesses> _observable;

        public MainWindow() =>
            InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _keyboardHook = new KeyboardHook();
            _observable   = _keyboardHook.ObservableKeys.Zip(ProcessObservable, (k, ps) => new KeyProcesses(k, ps)).Publish();
            _subscription = StableCompositeDisposable.Create(
                _observable.Connect(),
                _observable.Subscribe(kps =>
                {
                    var (key, processes) = kps;
                    foreach (var process in processes)
                    {
                        User32.PostMessage(process.MainWindowHandle, WM_CHAR, key, IntPtr.Zero);
                    }
                }),
                _observable.ObserveOn(new DispatcherSynchronizationContext()).Subscribe(kps =>
                {
                    var (key, processes) = kps;
                    foreach (var process in processes)
                    {
                        KeyText.Text += $"{key} to {process.Info()}{Environment.NewLine}";
                    }
                }));
            _keyboardHook.HookKeyboard();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _keyboardHook.UnHookKeyboard();
            _subscription.Dispose();
        }
        
        private static IObservable<List<Process>> ProcessObservable =>
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .Repeat()
                      .Select(_ => Process.GetProcesses().AsEnumerable()
                                          .Where(p => p.ProcessName == GameName.WithoutWhitespace())
                                          .Where(p => p.CharacterName() != GameName)
                                          .ToList());
    }
}
