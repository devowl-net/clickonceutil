using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// Folder information methods.
    /// </summary>
    public static class ClickOnceFolderInfoUtils
    {
        /// <summary>
        /// Check folder ClickOnce state.
        /// </summary>
        /// <param name="path">Folder path.</param>
        /// <returns>ClickOnce or not folder.</returns>
        public static bool IsClickOnceApplicationFolder(string path)
        {
            return false;

            //Directory.GetFiles(path)
        }
    }
}
