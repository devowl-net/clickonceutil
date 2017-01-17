using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using ClickOnceUtil4UI.Annotations;
using ClickOnceUtil4UI.Utils;

namespace ClickOnceUtil4UI.Windows.ChooseDialog
{
    // TODO MVVM
    /// <summary>
    /// Interaction logic for ChooseFolderDialog.xaml
    /// </summary>
    public partial class ChooseFolderDialog : INotifyPropertyChanged
    {
        private string _pathErrorText;

        private string _sourcePath;

        private string _selectedDrive;

        private bool _isInitializing = true;

        private string _selectedFolderName;

        private ClickOnceFolderInfo _selectedFolder;

        /// <summary>
        /// Создание экземпляра класса <see cref="ChooseFolderDialog"/>.
        /// </summary>
        public ChooseFolderDialog(string sourcePath)
        {
            DataContext = this;
            InitializeComponent();
            Initialize();
            SourcePath = sourcePath;
            _isInitializing = false;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Local computer logical drives.
        /// </summary>
        public ObservableCollection<string> Drives { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Current directory folders list.
        /// </summary>
        public ObservableCollection<ClickOnceFolderInfo> FoldersList { get; } =
            new ObservableCollection<ClickOnceFolderInfo>();

        /// <summary>
        /// Selected folder from the list.
        /// </summary>
        public ClickOnceFolderInfo SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
                SelectedFolderChanged();
            }
        }

        private void SelectedFolderChanged()
        {
            if (_selectedFolder != null)
            {
                SelectedFolderName = _selectedFolder.Name;
            }
        }

        /// <summary>
        /// History help items under path.
        /// </summary>
        public ObservableCollection<HistoryHelp> HistoryHelpItems { get; } = new ObservableCollection<HistoryHelp>();

        /// <summary>
        /// Chosen drive.
        /// </summary>
        public string SelectedDrive
        {
            get
            {
                return _selectedDrive;
            }
            set
            {
                _selectedDrive = value;
                SourcePath = value;
            }
        }

        /// <summary>
        /// Error image ToolTip text.
        /// </summary>
        public string PathErrorText
        {
            get
            {
                return _pathErrorText;
            }

            private set
            {
                _pathErrorText = value;
                OnPropertyChanged(nameof(PathErrorText));
                if (!string.IsNullOrEmpty(_pathErrorText) && !_isInitializing)
                {
                    ErrorToolTip.IsOpen = true;
                }
            }
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
            set
            {
                _sourcePath = (value ?? string.Empty);
                SourcePathChanged();
            }
        }

        /// <summary>
        /// Selected folder name.
        /// </summary>
        public string SelectedFolderName
        {
            get
            {
                return _selectedFolderName;
            }
            set
            {
                _selectedFolderName = value;
                SelectedFolderNameChanged();
                OnPropertyChanged(nameof(SelectedFolderName));
            }
        }

        private void SelectedFolderNameChanged()
        {
            if (Directory.Exists(SelectedFolderName))
            {
                SourcePath = SelectedFolderName;
                _selectedFolderName = string.Empty;
            }

            // Relevant Path not works
            //else
            //{
            //    // Path.Combine for "C:\123\abc" & "abc" gives "C:\123\abc" except "C:\123\abc\abc"
            //    //var relevantPath = Path.Combine(SourcePath, SelectedFolderName);
            //    var relevantPath = $@"{SourcePath}\{SelectedFolderName}";
            //    if (Directory.Exists(relevantPath))
            //    {
            //        SourcePath = relevantPath;
            //        _selectedFolderName = string.Empty;
            //    }
            //}
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void SourcePathChanged()
        {
            string validationResult;

            if (!PathUtils.IsFolderPathValid(SourcePath, out validationResult))
            {
                PathErrorText = validationResult;
                return;
            }

            PathErrorText = null;
            FoldersList.Clear();
            foreach (var folderPath in Directory.GetDirectories(SourcePath))
            {
                FoldersList.Add(new ClickOnceFolderInfo(folderPath));
            }
        }

        private void Initialize()
        {
            var phisicalDrives =
                DriveInfo.GetDrives()
                    .Where(drive => drive.DriveType == DriveType.Fixed)
                    .Select(drive => drive.Name)
                    .ToArray();

            foreach (var drive in phisicalDrives)
            {
                Drives.Add(drive);
            }

            var helpPairs = new[]
            {
                new { Desr = "Common folder", FolderType = FolderTypes.CommonFolder },
                new { Desr = "ClickOnce application", FolderType = FolderTypes.ClickOnceApplication },
                new { Desr = "Unknown ClickOnce application", FolderType = FolderTypes.UnknownClickOnceApplication },
                new { Desr = "Folder can be an application", FolderType = FolderTypes.CanBeAnApplication },
            };

            foreach (var pair in helpPairs)
            {
                HistoryHelpItems.Add(new HistoryHelp(pair.Desr, pair.FolderType));
            }
        }
        
        private void SelectedFolderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && SelectedFolder != null)
            {
                SourcePath = SelectedFolder.FullPath;
            }
        }

        private void UpperFolder(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(SourcePath))
            {
                var parent = Directory.GetParent(SourcePath);
                if (parent != null)
                {
                    SourcePath = parent.FullName;
                }
            }
        }
    }
}