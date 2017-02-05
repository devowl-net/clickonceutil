using System.Windows;

using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Prism;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private ClickOnceFolderInfo _selectedFolder;

        public MainWindowViewModel()
        {
            ChooseCommand = new DelegateCommand(ChooseHandler);
        }

        public DelegateCommand ChooseCommand { get; private set; }

        /// <summary>
        /// Folder source path.
        /// </summary>
        public ClickOnceFolderInfo SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }

            private set
            {
                _selectedFolder = value;
                RaisePropertyChanged(() => SelectedFolder);
            }
        }

        private void ChooseHandler(object obj)
        {
            var dataContext = new ChooseFolderDialogViewModel(SelectedFolder?.FullPath);
            var dialog = new ChooseFolderDialog(dataContext) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SelectedFolder = dataContext.SelectedFolder;
            }
        }
    }
}