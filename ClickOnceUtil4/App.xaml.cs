using System;
using System.Windows;
using System.Windows.Threading;

namespace ClickOnceUtil4UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Constructor for  <see cref="App"/>.
        /// </summary>
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.ToString());
            args.Handled = true;
        }
    }
}
