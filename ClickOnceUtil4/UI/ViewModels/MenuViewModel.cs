using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Prism;
using System.Windows;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// View model for <see cref="MainWindow"/> menu.
    /// </summary>
    public class MenuViewModel
    {
        private readonly MainWindowViewModel _mainViewModel;

        /// <summary>
        /// Constructor for <see cref="MenuViewModel"/>.
        /// </summary>
        public MenuViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            AboutCommand = new DelegateCommand(AboutHandler);
        }

        private void AboutHandler(object obj)
        {
            new AboutWindow { Owner = Application.Current.MainWindow }.ShowDialog();
        }

        /// <summary>
        /// About command handler.
        /// </summary>
        public DelegateCommand AboutCommand { get; private set; }
    }
}