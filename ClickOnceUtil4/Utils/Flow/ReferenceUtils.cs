using System;
using System.IO;
using System.Linq;
using System.Reflection;

using ClickOnceUtil4UI.Clickonce;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Methods for work with manifest references.
    /// </summary>
    public static class ReferenceUtils
    {
        /// <summary>
        /// Add required files from root directory.
        /// </summary>
        /// <param name="container">Container object.</param>
        public static void AddAssemblyReferences(Container container)
        {
            string root = container.FullPath;
            ApplicationManifest application = container.Application;

            // Cleaning references
            application.AssemblyReferences.Clear();
            application.FileReferences.Clear();

            // CommonLanguageRuntime (this reference came from VS exported application)
            var commonLanguageRuntime = new AssemblyReference()
            {
                AssemblyIdentity = new AssemblyIdentity("Microsoft.Windows.CommonLanguageRuntime", "4.0.30319.0"),
                IsPrerequisite = true
            };

            application.AssemblyReferences.Add(commonLanguageRuntime);

            // Add all files references
            InternalAddAssemblyReferences(application, root, root);

            application.ResolveFiles();
            application.UpdateFileInfo(application.TargetFrameworkVersion);
        }

        /// <summary>
        /// Get normal file path, just removes .deploy extension if it is.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>Normalized path.</returns>
        public static string GetNormalFilePath(string path)
        {
            if (!string.IsNullOrEmpty(Constants.DeployDotExtension) &&
                Path.GetExtension(path) == Constants.DeployDotExtension)
            {
                return path.Substring(0, path.Length - Constants.DeployDotExtension.Length);
            }

            return path;
        }

        private static void InternalAddAssemblyReferences(ApplicationManifest application, string currentDirectory, string root)
        {
            foreach (var file in Directory.GetFiles(currentDirectory))
            {
                var filePath = file;
                if (Path.GetExtension(filePath) == Constants.DeployDotExtension)
                {
                    File.Move(filePath, filePath = GetNormalFilePath(filePath));
                }

                var fileExtension = Path.GetExtension(filePath);

                if (fileExtension != null &&
                    Constants.IgnoreReferences.Any(
                        item => string.Equals(item, fileExtension, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                BaseReference fileReference;
                try
                {
                    AssemblyName.GetAssemblyName(filePath);
                    AssemblyReference assemblyReference;
                    fileReference = assemblyReference = application.AssemblyReferences.Add(filePath);
                    assemblyReference.AssemblyIdentity = AssemblyIdentity.FromFile(filePath);
                }
                catch (BadImageFormatException)
                {
                    fileReference = application.FileReferences.Add(filePath);
                }

                fileReference.TargetPath = PathUtils.GetRelativePath(filePath, root);
            }

            foreach (var directory in Directory.GetDirectories(currentDirectory))
            {
                InternalAddAssemblyReferences(application, directory, root);
            }
        }
    }
}