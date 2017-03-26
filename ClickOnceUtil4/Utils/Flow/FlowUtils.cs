using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using ClickOnceUtil4UI.Clickonce;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Web.Administration;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Methods for building application in flows.
    /// </summary>
    public static class FlowUtils
    {
        /// <summary>
        /// Create default <see cref="DeployManifest"/>.
        /// </summary>
        /// <param name="root">Root folder path.</param>
        /// <param name="entrypoint">Endpoint file name.</param>
        /// <returns>Returns <see cref="DeployManifest"/> instance.</returns>
        public static DeployManifest CreateDeployManifest(string root, string entrypoint)
        {
            var fileName = $"{Path.GetFileNameWithoutExtension(entrypoint)}.{Constants.ApplicationExtension}";
            return new DeployManifest($".NETFramework,Version={Constants.DefaultFramework}")
            {
                SourcePath =
                    Path.Combine(
                        root,
                        fileName),
                Publisher = "Publisher",
                Product = "Product",
                MapFileExtensions = true,
                UpdateMode = UpdateMode.Foreground,
                UpdateEnabled = true,
                DeploymentUrl = GetDeployUrl(root, fileName)
            };
        }
        
        /// <summary>
        /// Get deploy file url.
        /// </summary>
        /// <param name="root">Root folder path.</param>
        /// <param name="applicationFileName">Application file name.</param>
        /// <returns></returns>
        public static string GetDeployUrl(string root, string applicationFileName)
        {
            foreach (var site in new ServerManager().Sites)
            {
                foreach (var application in site.Applications)
                {
                    foreach (VirtualDirectory directory in application.VirtualDirectories)
                    {
                        if (root.StartsWith(directory.PhysicalPath))
                        {
                            var protocols = new[]
                            {
                                "http",
                                "https"
                            };

                            var binding = site.Bindings.FirstOrDefault(b => protocols.Contains(b.Protocol));
                            if (binding != null)
                            {
                                string protocol = binding.Protocol;
                                string domainName = Environment.MachineName;
                                if (!Equals(binding.EndPoint.Address, IPAddress.Any))
                                {
                                    domainName = binding.EndPoint.Address.ToString();
                                }

                                var deployUrl =
                                    $"{protocol}://{domainName}{directory.Path}{root.Substring(directory.PhysicalPath.Length).Replace("\\", "/")}/{applicationFileName}";

                                return deployUrl;
                            }
                        }
                    }
                }
            }

            return $"http://domain/subfolder/{applicationFileName}";
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
                OSVersion = "4.10.0.0",

                TargetFrameworkVersion = Constants.DefaultFramework
            };
        }

        /// <summary>
        /// Clean applications cache.
        /// </summary>
        [DllImport("Dfshim.dll", CharSet = CharSet.Auto, ExactSpelling = false)]
        public static extern void CleanOnlineAppCache();

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
        /// Rename .deploy files, just remove extension.
        /// </summary>
        /// <param name="path">Directory path.</param>
        public static void AddDeployExtention(string path)
        {
            var deployExtension = $".{Constants.DeployFileExtension}";
            RecursiveDirectoryWalker(
                path,
                fileName =>
                    Path.GetExtension(fileName) != deployExtension &&
                    !Constants.IgnoreReferences.Any(
                        item => string.Equals(item, Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// Get target framework.
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <returns>Target framework.</returns>
        public static string GetTargetFramework(Container container)
        {
            return container.Application.TargetFrameworkVersion;
        }

        /// <summary>
        /// Set global important settings.
        /// </summary>
        /// <param name="manifest">Manifest file reference.</param>
        /// <param name="container">Container instance.</param>
        public static void SetGlobals(Manifest manifest, Container container)
        {
            var targetFramewrok = GetTargetFramework(container);

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
            manifest.UpdateFileInfo(targetFramewrok);
        }

        /// <summary>
        /// Reads ClickOnce application version.
        /// </summary>
        /// <param name="deploy"><see cref="DeployManifest"/> file instance.</param>
        /// <returns>Version inside <see cref="DeployManifest"/>.</returns>
        public static string ReadApplicationVersion(DeployManifest deploy)
        {
            return deploy.AssemblyIdentity.Version;
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
        /// Reads ClickOnce application name.
        /// </summary>
        /// <param name="application"><see cref="ApplicationManifest"/> instance.</param>
        /// <returns>Application Name.</returns>
        public static string ReadApplicationName(ApplicationManifest application)
        {
            return application.AssemblyIdentity.Name ?? application.EntryPoint?.AssemblyIdentity?.Name ?? Path.GetFileNameWithoutExtension(application.SourcePath);
        }
    }
}