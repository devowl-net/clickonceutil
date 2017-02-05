using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils;
using ClickOnceUtil4UI.Utils.Prism;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// <see cref="ChooseFolderDialog"/> view model.
    /// </summary>
    public class ChooseFolderDialogViewModel : NotificationObject
    {
        private string _pathErrorText;

        private string _sourcePath;

        private string _selectedDrive;
        
        private string _selectedFolderName;

        private ClickOnceFolderInfo _selectedFolder;

        /// <summary>
        /// Создание экземпляра класса <see cref="ChooseFolderDialog"/>.
        /// </summary>
        public ChooseFolderDialogViewModel(string sourcePath)
        {
            UpperFolderCommand = new DelegateCommand(UpperFolderHandler);
            SelectedFolderDoubleClickCommand = new DelegateCommand(SelectedFolderDoubleClickHandler);
            SelectFolderCommand = new DelegateCommand(SelectFolderHandler, CanSelectFolder);
            RefreshFolderCommand = new DelegateCommand(RefreshFolderHandler);
            Initialize();
            SourcePath = sourcePath;
        }

        private bool CanSelectFolder(object obj)
        {
            return SelectedFolder != null &&
                   (SelectedFolder.FolderType == FolderTypes.CanBeAnApplication ||
                    SelectedFolder.FolderType == FolderTypes.ClickOnceApplication);
        }

        /// <summary>
        /// Folder double click command.
        /// </summary>
        public DelegateCommand SelectedFolderDoubleClickCommand { get; private set; }

        /// <summary>
        /// Select folder command.
        /// </summary>
        public DelegateCommand SelectFolderCommand { get; private set; }

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
                RaisePropertyChanged(() => SelectedFolder);
                SelectedFolderChanged();
                SelectFolderCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Upper folder click command.
        /// </summary>
        public DelegateCommand UpperFolderCommand { get; private set; }

        /// <summary>
        /// Upper folder click command.
        /// </summary>
        public DelegateCommand RefreshFolderCommand { get; private set; }

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
        /// Error ToolTip text.
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
                RaisePropertyChanged(() => PathErrorText);
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
                RaisePropertyChanged(() => SelectedFolderName);
            }
        }

        private void RefreshFolderHandler(object obj)
        {
            foreach (var folder in FoldersList)
            {
                folder.Update();
            }
        }

        private void SelectFolderHandler(object obj)
        {
            Window window = (Window)obj;
            if (window != null)
            {
                window.DialogResult = true;
            }
        }

        private void UpperFolderHandler(object obj)
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

        private void SelectedFolderChanged()
        {
            if (_selectedFolder != null)
            {
                SelectedFolderName = _selectedFolder.Name;
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
                new { Desr = "Folder can be an application", FolderType = FolderTypes.CanBeAnApplication },
                new { Desr = "Unknown ClickOnce application", FolderType = FolderTypes.UnknownClickOnceApplication },
                new { Desr = "No access", FolderType = FolderTypes.HaveProblems }
            };

            foreach (var pair in helpPairs)
            {
                HistoryHelpItems.Add(new HistoryHelp(pair.Desr, pair.FolderType));
            }
        }

        private void SelectedFolderDoubleClickHandler(object obj)
        {
            if (SelectedFolder != null)
            {
                SourcePath = SelectedFolder.FullPath;
            }
        }
    }
}