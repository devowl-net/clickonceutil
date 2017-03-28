using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.Utils.EmbeddedManifests;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Methods for getting info about manifests.
    /// </summary>
    public static class InfoUtils
    {
        /// <summary>
        /// Reads important information from <see cref="DeployManifest"/>.
        /// </summary>
        /// <param name="deploy"><see cref="DeployManifest"/> instance.</param>
        /// <returns><see cref="InfoData"/> items.</returns>
        public static IEnumerable<InfoData> GetDeployInfoData(DeployManifest deploy)
        {
            var fileName = Path.GetFileName(deploy.SourcePath);
            var urlFileName = GetUrlFileName(deploy.DeploymentUrl);


            if (!string.Equals(fileName, urlFileName, StringComparison.OrdinalIgnoreCase))
            {
                yield return new InfoData(nameof(deploy.SourcePath), "SourcePath application file name is not equals to DeploymentUrl file name.", true);
            }

            yield return
                new InfoData(
                    nameof(deploy.DeploymentUrl),
                    $"Your application hosting service must publishing your folder: \"{Path.GetDirectoryName(deploy.SourcePath)}\" and clients will try to activate your application from URI: {deploy.DeploymentUrl}");

            yield return
                new InfoData(
                    nameof(deploy.MapFileExtensions),
                    deploy.MapFileExtensions
                        ? "All application files will be renamed from [file.ext] to [file.ext.deploy]. Its useful if your publishing service have download files restrictions."
                        : "Be sure that your publishing service have not file restrictions for your files.");

            if (deploy.TrustUrlParameters)
            {
                yield return
                    new InfoData(
                        nameof(deploy.TrustUrlParameters),
                        "Any URL activation parameters will be passed to your application entry point as command line arguments.")
                    ;
            }

            if (!deploy.UpdateEnabled)
            {
                yield return new InfoData(nameof(deploy.UpdateEnabled), "Your application will not be updatable.", true);
            }

            yield return
                new InfoData(
                    nameof(deploy.UpdateMode),
                    deploy.UpdateMode == UpdateMode.Foreground
                        ? "Your application updating before launch."
                        : "Your application starts with a current copy and downloading new copy while works.");
        }

        private static string GetUrlFileName(string deploymentUrl)
        {
            if (string.IsNullOrEmpty(deploymentUrl))
            {
                return string.Empty;
            }

            var buffer = new StringBuilder(); 
            var pointer = deploymentUrl.Length - 1;
            while (pointer >= 0 && !new[] { '\\', '/' }.Contains(deploymentUrl[pointer]))
            {
                buffer.Append(deploymentUrl[pointer]);
                pointer --;
            }

            return new string(buffer.ToString().Reverse().ToArray());
        }

        /// <summary>
        /// Reads important information from <see cref="ApplicationManifest"/>.
        /// </summary>
        /// <param name="application"><see cref="ApplicationManifest"/> instance.</param>
        /// <returns><see cref="InfoData"/> items.</returns>
        public static IEnumerable<InfoData> GetApplicationInfoData(ApplicationManifest application)
        {
            if (!application.IsClickOnceManifest)
            {
                yield return new InfoData(nameof(application.IsClickOnceManifest), "Your are going to create ClickOnceApplication for none managed Win32 application.", true);
            }

            if (string.IsNullOrWhiteSpace(application.SuiteName))
            {
                yield return new InfoData(nameof(application.SuiteName), "No name of the folder on the Start menu where the application is located after ClickOnce deployment. Your name will be application name.", true);
            }
        }

        private static bool HasEmbeddedManifestInsideExecutableAssemblies(Manifest application, out string assemblyNames)
        {
            bool hasAssemblies = false;
            var assemblies = new List<string>();
            foreach (AssemblyReference assembly in application.AssemblyReferences)
            {
                if (Path.GetExtension(assembly.SourcePath) == $".{Constants.ExecutableFileExtension}" &&
                    EmbeddedManifest.Read(assembly.SourcePath) != null)
                {
                    assemblies.Add(Path.GetFileName(assembly.SourcePath));
                    hasAssemblies = true;
                }
            }

            assemblyNames = string.Join(", ", assemblies);
            return hasAssemblies;
        }

        private static bool HasStrongNameExecutableAssemblies(Manifest application, out string assemblyNames)
        {
            bool hasAssemblies = false;
            var assemblies = new List<string>();
            foreach (AssemblyReference assembly in application.AssemblyReferences)
            {
                if (Path.GetExtension(assembly.SourcePath) == $".{Constants.ExecutableFileExtension}" &&
                    assembly.AssemblyIdentity.IsStrongName)
                {
                    assemblies.Add(Path.GetFileName(assembly.SourcePath));
                    hasAssemblies = true;
                }
            }

            assemblyNames = string.Join(", ", assemblies);
            return hasAssemblies;
        }

        /// <summary>
        /// Check filling of required for generation fields.
        /// </summary>
        /// <param name="container">Objects container.</param>
        /// <param name="errorString">Contains error string.</param>
        /// <returns></returns>
        public static bool IsRequiredFieldsFilled(Container container, out string errorString)
        {
            errorString = null;
            var deploy = container.Deploy;

            if (string.IsNullOrEmpty(deploy.DeploymentUrl) ||
                !deploy.DeploymentUrl.EndsWith(Constants.ApplicationExtension))
            {
                errorString =
                    "[DeploymentUrl] parameter should have a URL (example: http(s)://site/appfilename.application) to your published file.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate <see cref="Manifest"/>.
        /// </summary>
        /// <param name="manifest">Reference to <see cref="Manifest"/>.</param>
        /// <param name="errorString">Error text.</param>
        /// <returns>Is valid or not.</returns>
        public static bool IsValidManifest(Manifest manifest, out string errorString)
        {
            errorString = null;
            manifest.Validate();

            if (manifest.OutputMessages.ErrorCount > 0)
            {
                errorString =
                    $"{manifest.GetType().Name} errors:{Environment.NewLine + Environment.NewLine}{ReadOutputMessages(manifest.OutputMessages)}";
                return false;
            }
            
            string assemblies;
            if (HasStrongNameExecutableAssemblies(manifest, out assemblies))
            {
                // https://msdn.microsoft.com/en-us/library/aa730868(v=vs.80).aspx
                errorString = $"You have a Strong namged EXE files ({assemblies}) its not allowed, unless it will be deployed to GAC. Read information from here https://msdn.microsoft.com/en-us/library/aa730868(v=vs.80).aspx (ClickOnce Manifest Signing and Strong-Name Assembly Signing Using Visual Studio Project Designer's Signing Page)";

                return false;
            }

            if (HasEmbeddedManifestInsideExecutableAssemblies(manifest, out assemblies))
            {
                errorString = $"You have assemblies ({assemblies}) contains embedded manifest information, its unacceptable and will be cause of error:"
                    + Environment.NewLine + Environment.NewLine +
                    "+ Reference in the manifest does not match the identity of the downloaded assembly [AppName.exe]"
                    + Environment.NewLine + Environment.NewLine +
                    @"So just go to [Project property] -> [Application tab] -> and change [Embed manifest with default settings] to [Create application without a manifest]."
                    + Environment.NewLine +
                    "If you have not access to source code then you can use [mt.exe] utility by Microsoft for adding or modification of assemblies. BTW the problem source is <assemblyIdentity /> tag of manifest (Didn't try to remove).";

                return false;
            }

            return true;
        }

        private static StringBuilder ReadOutputMessages(OutputMessageCollection outputMessages)
        {
            var buffer = new StringBuilder();
            int counter = 1;

            foreach (OutputMessage outputMessage in outputMessages)
            {
                buffer.AppendFormat($"{counter}) {outputMessage.Text}");
                buffer.AppendLine();
                counter++;
            }

            return buffer;
        }
    }
}