using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
        public ObservableCollection<ClickOnceFolderInfo> FoldersList { get; } = new ObservableCollection<ClickOnceFolderInfo>(); 

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

        private void SourcePathChanged()
        {
            string validationResult;
            
            ColorizePath();
            if (!PathUtils.IsFolderPathValid(SourcePath, out validationResult))
            {
                PathErrorText = validationResult;
                return;
            }

            PathErrorText = null;
            foreach (var folderPath in Directory.GetDirectories(SourcePath))
            {
                FoldersList.Add(new ClickOnceFolderInfo(folderPath));
            }

        }

        private FlowDocument SelectedFolderDocument => SelectedFolder.Document;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        
        private void ColorizePath()
        {
            SelectedFolder.Document.Blocks.Clear();
            if (string.IsNullOrEmpty(SourcePath))
            {
                return;
            }

            var directoryName = Path.GetDirectoryName(SourcePath) ?? string.Empty;
            var prefixPath = SourcePath.Substring(0, SourcePath.Length - directoryName.Length);
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run(prefixPath));
            paragraph.Inlines.Add(new Run(directoryName) { Foreground = Brushes.ForestGreen });

            SelectedFolder.Document.Blocks.Add(paragraph);
        }

        private void SelectedFolderTextChanged(object sender, KeyEventArgs e)
        {
            SourcePath = new TextRange(SelectedFolderDocument.ContentStart, SelectedFolderDocument.ContentEnd).Text.Replace(Environment.NewLine, string.Empty);
        }
    }
}