using System.ComponentModel;
using System.Windows;
using System;

namespace Ao.Auto.Ui
{
    public partial class MainWindow : Window
    {
        private KeyboardHook _keyboardHook;
        private IDisposable  _subscription;
        
        public MainWindow() =>
            InitializeComponent();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _keyboardHook = new KeyboardHook();
            _subscription = _keyboardHook.ObservableKeys.Subscribe(key => KeyText.Text += key);
            _keyboardHook.HookKeyboard();
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            _keyboardHook.UnHookKeyboard();
            _subscription.Dispose();
        }
    }
}
