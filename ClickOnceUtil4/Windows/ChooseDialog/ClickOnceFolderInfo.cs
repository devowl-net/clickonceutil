using System.IO;
using System.Windows.Controls;

namespace ClickOnceUtil4UI.Windows.ChooseDialog
{
    /// <summary>
    /// ClickOnce information about folder.
    /// </summary>
    public class ClickOnceFolderInfo
    {
        private TextBlock _folderInfo;

        /// <summary>
        /// Создание экземпляра класса <see cref="ClickOnceFolderInfo"/>.
        /// </summary>
        public ClickOnceFolderInfo(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
        }
        
        /// <summary>
        /// Folder Type value.
        /// </summary>
        public FolderTypes FolderType
        {
            get
            {
                return  FolderTypes.CommonFolder;
            }
        }

        /// <summary>
        /// Icon description.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Full path.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Formatted selected folder info.
        /// </summary>
        public TextBlock FolderInfo
        {
            get
            {
                return _folderInfo ?? (_folderInfo = CreateFolderInfo());
            }
        }

        private TextBlock CreateFolderInfo()
        {
            return null;
        }
    }
}