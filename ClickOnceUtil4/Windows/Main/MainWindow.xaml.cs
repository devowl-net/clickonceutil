using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using ClickOnceUtil4UI.Annotations;
using ClickOnceUtil4UI.Windows.ChooseDialog;

namespace ClickOnceUtil4UI.Windows.Main
{
    // TODO MVVM
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _sourcePath;

        /// <summary>
        /// Создание экземпляра класса <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

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
                OnPropertyChanged(nameof(SourcePath));
            }
        }

        private void ChooseClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ChooseFolderDialog(SourcePath) { Owner = this };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SourcePath = dialog.SourcePath;
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}