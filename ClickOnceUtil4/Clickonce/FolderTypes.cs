using System.ComponentModel;

namespace ClickOnceUtil4UI.Clickonce
{
    /// <summary>
    /// Folder types.
    /// </summary>
    public enum FolderTypes
    {
        /// <summary>
        /// Common folder.
        /// </summary>
        [Description("Common folder")]
        CommonFolder,

        /// <summary>
        /// Folder contains ClickOnce application.
        /// </summary>
        [Description("ClickOnce application")]
        ClickOnceApplication,

        /// <summary>
        /// Unknown type of application or manifest files.
        /// </summary>
        [Description("Folder can be an application")]
        UnknownClickOnceApplication,

        /// <summary>
        /// Folder can be chosen as target for ClickOnce application.
        /// </summary>
        [Description("Unknown ClickOnce application")]
        CanBeAnApplication,

        /// <summary>
        /// Have some problems.
        /// </summary>
        [Description("Access problems")]
        HaveProblems
    }
}
