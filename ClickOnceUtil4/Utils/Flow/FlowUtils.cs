using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Automation.Peers;

using ClickOnceUtil4UI.Clickonce;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Methods for building application in flows.
    /// </summary>
    public static class FlowUtils
    {
        private static readonly IEnumerable<string> IgnoreReferences = new[]
        {
            // .application
            $".{Constants.ApplicationExtension}",

            // .manifest
            $".{Constants.ManifestExtension}"
        };

        private static readonly string DeployDotExtension = $".{Constants.DeployFileExtension}";

        /// <summary>
        /// Create default <see cref="DeployManifest"/>.
        /// </summary>
        /// <param name="root">Root folder path.</param>
        /// <param name="entrypoint">Endpoint file name.</param>
        /// <returns>Returns <see cref="DeployManifest"/> instance.</returns>
        public static DeployManifest CreateDeployManifest(string root, string entrypoint)
        {
            var @return = new DeployManifest($".NETFramework,Version={Constants.DefaultFramework}")
            {
                SourcePath = Path.Combine(root, $"{Path.GetFileNameWithoutExtension(entrypoint)}.{Constants.ApplicationExtension}"),
                Publisher = "Publisher",
                Product = "Product",
                MapFileExtensions = true,
                UpdateMode = UpdateMode.Foreground,
                UpdateEnabled = true
            };

            // TODO DELETE
            @return.DeploymentUrl = "http://localhost/IISRoot/DELME/ClickOnceGen.application";
            return @return;
        }

        /// <summary>
        /// Create default <see cref="ApplicationManifest"/>.
        /// </summary>
        /// <param name="root">Root folder path.</param>
        /// <param name="entrypoint">Endpoint file name.</param>
        /// <returns>Returns <see cref="ApplicationManifest"/> instance.</returns>
        public static ApplicationManifest CreateApplicationManifest(string root, string entrypoint)
        {
            return new ApplicationManifest(Constants.DefaultFramework)
            {
                SourcePath = Path.Combine(root, $"{entrypoint}.{Constants.ManifestExtension}"),

                // Windows XP
                OSVersion = "4.10.0.0"
            };
        }

        /// <summary>
        /// Sign file with certificate.
        /// </summary>
        /// <param name="manifest">Manifest file reference.</param>
        /// <param name="certificate">Certificate file reference.</param>
        public static void SignFile(Manifest manifest, X509Certificate2 certificate)
        {
            SecurityUtilities.SignFile(certificate, null, manifest.SourcePath);
        }

        /// <summary>
        /// Add required files from root directory.
        /// </summary>
        /// <param name="application"><see cref="ApplicationManifest"/> file reference.</param>
        /// <param name="root">Root path to directory.</param>
        public static void AddReferences(ApplicationManifest application, string root)
        {
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
            InternalAddReferences(application, root, root);

            application.ResolveFiles();
            application.UpdateFileInfo(Constants.DefaultFramework);
        }

        /// <summary>
        /// Get normal file path, just removes .deploy extension if it is.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <returns>Normalized path.</returns>
        public static string GetNormalFilePath(string path)
        {
            if (Path.GetExtension(path) == DeployDotExtension)
            {
                return path.Substring(0, path.Length - DeployDotExtension.Length);
            }

            return path;
        }

        private static void InternalAddReferences(ApplicationManifest application, string currentDirectory, string root)
        {
            foreach (var file in Directory.GetFiles(currentDirectory))
            {
                var filePath = file;
                if (Path.GetExtension(filePath) == DeployDotExtension)
                {
                    File.Move(filePath, filePath = GetNormalFilePath(filePath));
                }

                var fileExtension = Path.GetExtension(filePath);

                if(fileExtension != null && IgnoreReferences.Any(item => string.Equals(item, fileExtension, StringComparison.InvariantCultureIgnoreCase)))
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
                InternalAddReferences(application, directory, root);
            }
        }
        /// <summary>
        /// Rename .deploy files, just remove extension.
        /// </summary>
        /// <param name="path">Directory path.</param>
        public static void AddDeployExtention(string path)
        {
            var deployExtension = $".{Constants.DeployFileExtension}";
            RecursiveDirectoryWalker(
                path,
                fileName =>
                    Path.GetExtension(fileName) != deployExtension && !IgnoreReferences.Any(item => string.Equals(item, Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
                        ? $"{fileName}{deployExtension}"
                        : fileName);
        }

        /// <summary>
        /// Rename .deploy files, just remove extension.
        /// </summary>
        /// <param name="path">Directory path.</param>
        public static void RemoveDeployExtention(string path)
        {
            var deployExtension = $".{Constants.DeployFileExtension}";
            RecursiveDirectoryWalker(
                path,
                fileName =>
                    Path.GetExtension(fileName) == deployExtension
                        ? Path.GetFileNameWithoutExtension(fileName)
                        : fileName);
        }

        private static void RecursiveDirectoryWalker(string path, Func<string, string> action)
        {
            var currentDirectory = new DirectoryInfo(path);
            foreach (var file in currentDirectory.GetFiles())
            {
                var fileName = file.Name;
                var newName = action(fileName);
                if (newName != fileName)
                {
                    var fullNewName = Path.Combine(path, newName);
                    File.Move(file.FullName, fullNewName);
                }
            }

            foreach (var subDirectory in currentDirectory.GetDirectories())
            {
                RecursiveDirectoryWalker(subDirectory.FullName, action);
            }
        }

        /// <summary>
        /// Set global important settings.
        /// </summary>
        /// <param name="manifest">Manifest file reference.</param>
        public static void SetGlobals(Manifest manifest)
        {
            /*
                * Activation of http://localhost/IISRoot/DELME/Launcher.application resulted in exception. Following failure messages were detected:
		            + Exception reading manifest from http://localhost/IISRoot/DELME/Launcher.application: the manifest may not be valid or the file could not be opened.
		            + Deployment manifest is not semantically valid.
		            + Deployment manifest identity contains missing or unsupported processor architecture.

                My current OS is windows10 x64, so x86 marks did a bad things
             */
            const string Architecture = "msil";
            const string Language = "neutral";

            if (manifest.AssemblyIdentity != null)
            {
                manifest.AssemblyIdentity.ProcessorArchitecture = Architecture;
                manifest.AssemblyIdentity.Culture = string.IsNullOrEmpty(manifest.AssemblyIdentity.Culture)
                    ? Language
                    : manifest.AssemblyIdentity.Culture;
            }

            foreach (AssemblyReference assembly in manifest.AssemblyReferences)
            {
                if (assembly.AssemblyIdentity != null)
                {
                    assembly.AssemblyIdentity.ProcessorArchitecture = Architecture;
                    assembly.AssemblyIdentity.Culture = string.IsNullOrEmpty(assembly.AssemblyIdentity.Culture)
                    ? Language
                    : assembly.AssemblyIdentity.Culture;
                }
            }

            manifest.ResolveFiles();
            manifest.UpdateFileInfo(Constants.DefaultFramework);
        }
    }
}
