namespace ClickOnceUtil4UI.Windows.ChooseDialog
{
    /// <summary>
    /// History model item.
    /// </summary>
    public class HistoryHelp
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="HistoryHelp"/>.
        /// </summary>
        public HistoryHelp(string description, FolderTypes folderType)
        {
            Description = description;
            FolderType = folderType;
        }

        /// <summary>
        /// Folder Type value.
        /// </summary>
        public FolderTypes FolderType { get; private set; }

        /// <summary>
        /// Icon description.
        /// </summary>
        public string Description { get; private set; }
    }
}