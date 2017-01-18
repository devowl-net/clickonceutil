using System.Windows;

using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Prism;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private string _sourcePath;

        public MainWindowViewModel()
        {
            ChooseCommand = new DelegateCommand(ChooseHandler);
        }

        public DelegateCommand ChooseCommand { get; private set; }

        /// <summary>
        /// Folder source path.
        /// </summary>
        public string SourcePath
        {
            get
            {
                return _sourcePath;
            }

            private set
            {
                _sourcePath = value;
                RaisePropertyChanged(() => SourcePath);
            }
        }

        private void ChooseHandler(object obj)
        {
            var dataContext = new ChooseFolderDialogViewModel(SourcePath);
            var dialog = new ChooseFolderDialog(dataContext) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SourcePath = dataContext.SourcePath;
            }
        }
    }
}