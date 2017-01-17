using System.IO;
using System.Windows.Controls;

namespace ClickOnceUtil4UI.Windows.ChooseDialog
{
    /// <summary>
    /// ClickOnce information about folder.
    /// </summary>
    public class ClickOnceFolderInfo
    {
        private string _path;

        private TextBlock _folderInfo;

        /// <summary>
        /// Создание экземпляра класса <see cref="ClickOnceFolderInfo"/>.
        /// </summary>
        public ClickOnceFolderInfo(string path)
        {
            _path = path;
            Name = Path.GetFileName(_path);
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