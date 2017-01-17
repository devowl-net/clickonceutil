using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClickOnceUtil4UI.Windows.ChooseDialog
{
    /// <summary>
    /// Folder types.
    /// </summary>
    public enum FolderTypes
    {
        /// <summary>
        /// Common folder.
        /// </summary>
        CommonFolder,

        /// <summary>
        /// Folder contains ClickOnce application.
        /// </summary>
        ClickOnceApplication,

        /// <summary>
        /// Unknown type of application or manifest files.
        /// </summary>
        UnknownClickOnceApplication,

        /// <summary>
        /// Folder can be chosen as target for ClickOnce application.
        /// </summary>
        CanBeAnApplication
    }
}
