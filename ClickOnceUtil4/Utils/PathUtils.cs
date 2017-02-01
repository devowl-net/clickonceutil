using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// Набор утилит для работы с путями.
    /// </summary>
    public static class PathUtils
    {
        private static readonly string[] IgnoredFolderNames = {
            "Documents and Settings",
            "System Volume Information",
            "$RECYCLE.BIN",
            "Users",
            "Windows"
        };

        private static readonly char[] InvalidDirectoryNameCharactors =
        {
            '<',
            '>',
            ':',
            '"',
            '/',
            '\\',
            '|',
            '?',
            '*'
        };
        
        /// <summary>
        /// Path validation.
        /// </summary>
        /// <param name="sourcePath">Path string.</param>
        /// <param name="validationResult">if something wrong, then print info here.</param>
        /// <returns></returns>
        public static bool IsFolderPathValid(string sourcePath, out string validationResult)
        {
            const string CommonError = "The path is incorrect.";

            validationResult = string.Empty;
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                validationResult = "The path is empty.";
                return false;
            }

            string rootName;
            try
            {
                rootName = Path.GetPathRoot(sourcePath);
                if (string.IsNullOrEmpty(rootName) || rootName.StartsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    validationResult = "Root drive is not chosen.";
                    return false;
                }
                
                if (sourcePath[rootName.Length - 1] != Path.DirectorySeparatorChar)
                {
                    validationResult = CommonError;
                    return false;
                }

                var subPath = new string(sourcePath.Skip(3).ToArray());
                foreach (var folderName in subPath.Split(
                    new[]
                    {
                        '\\'
                    },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    if (folderName.IndexOfAny(InvalidDirectoryNameCharactors) >= 0)
                    {
                        validationResult = "Path contains invalid characters.";
                        return false;
                    }
                }
            }
            catch (ArgumentException)
            {
                validationResult = CommonError;
                return false;
            }

            if (string.IsNullOrEmpty(rootName))
            {
                validationResult = "Path cannot be empty.";
                return false;
            }

            if (!CheckFolderWritePermissions(sourcePath))
            {
                validationResult = "No write permissions for chosen folder";
                return false;
            }
            
            if (
                !DriveInfo.GetDrives()
                    .Any(
                        drive => drive.Name.StartsWith(rootName) && drive.IsReady && drive.DriveType == DriveType.Fixed))
            {
                validationResult =
                    "Choose logical drive.";
                return false;
            }

            if (!Directory.Exists(sourcePath))
            {
                validationResult = "Folder doesn't exists.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check write to folder permissions.
        /// </summary>
        /// <param name="sourcePath">Path to folder.</param>
        /// <returns>Have or not access.</returns>
        public static bool CheckFolderReadPermissions(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            {
                return true;
            }

            var readAllow = false;
            var readDeny = false;
            var accessControlList = Directory.GetAccessControl(sourcePath);
            if (accessControlList == null)
            {
                return false;
            }

            var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    readAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    readDeny = true;
                }
            }

            return readAllow && !readDeny;
        }

        /// <summary>
        /// Check write to folder permissions.
        /// </summary>
        /// <param name="sourcePath">Path to folder.</param>
        /// <returns>Have or not access.</returns>
        public static bool CheckFolderWritePermissions(string sourcePath)
        {
            if (!Directory.Exists(sourcePath))
            {
                return true;
            }

            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = Directory.GetAccessControl(sourcePath);
            if (accessControlList == null)
            {
                return false;
            }

            var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }

        /// <summary>
        /// Is path in ignore list.
        /// </summary>
        /// <param name="fullPath">Folder path.</param>
        /// <returns>Is it or not.</returns>
        public static bool IsIgnoredPath(string fullPath)
        {
            var folderName = Path.GetFileName(fullPath);
            return IgnoredFolderNames.Any(badName => string.Equals(folderName, badName, StringComparison.OrdinalIgnoreCase));
        }
    }
}