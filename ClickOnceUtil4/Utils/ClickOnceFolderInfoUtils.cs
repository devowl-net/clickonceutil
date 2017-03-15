using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

using ClickOnceUtil4UI.Clickonce;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// Folder information methods.
    /// </summary>
    public static class ClickOnceFolderInfoUtils
    {
        /// <summary>
        /// Check single file existence.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileExtension"></param>
        /// <param name="filesCount"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool TryGetSingleFile(string path, string fileExtension, out int filesCount, out string filePath)
        {
            var files = Directory.GetFiles(path, $"*.{fileExtension}");
            filePath = null;
            filesCount = files.Length;
            bool isSingleFile = files.Length == 1;
            if (isSingleFile)
            {
                filePath = files.Single();
            }

            return isSingleFile;
        }

        /// <summary>
        /// Try to read .application or .manifest file.
        /// </summary>
        /// <typeparam name="TResult">Result manifest type.</typeparam>
        /// <param name="filePath">Path to file.</param>
        /// <param name="result">Manifest result.</param>
        /// <param name="error">Error string.</param>
        /// <returns>Is file read successful.</returns>
        public static bool TryReadClickOnceFile<TResult>(string filePath, out TResult result, out string error)
            where TResult : Manifest
        {
            result = null;
            error = null;

            try
            {
                result = (TResult)ManifestReader.ReadManifest(filePath, true);
            }
            catch (Exception ex)
            {
                error =
                    $"Unable to parse file '{Path.GetFileName(filePath)}' manifest. The error message is: {ex.Message}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Folder can to be the ClickOnce application.
        /// </summary>
        /// <param name="fullPath">Folder path.</param>
        /// <returns>Is it possible.</returns>
        public static bool IsFolderCanBeClickOnceApplication(string fullPath)
        {
            /*
             Requirements:
             1. Inside folder exists any executable file which one target is v4.0. (EntryPoint)
             2. (?) Every dll should be managed (?)
             */
            foreach (
                var executableFilePath in
                    Directory.GetFiles(fullPath, $"*.{Clickonce.Constants.ExecutableFileExtension}"))
            {
                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(executableFilePath);
                }
                catch (BadImageFormatException)
                {
                    // $"*
                    continue;
                }

                var q = assembly.ImageRuntimeVersion;
                TargetFrameworkAttribute targetFrameworkAttribute =
                    (TargetFrameworkAttribute)
                        assembly.GetCustomAttributes(typeof(TargetFrameworkAttribute), false).FirstOrDefault();

                // TargetFrameworkAttribute framework v4.0 only
                if (targetFrameworkAttribute != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}