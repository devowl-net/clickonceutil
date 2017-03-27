using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Contains utilities for manifest updates.
    /// </summary>
    public static class UpdateManifestUtils
    {
        /// <summary>
        /// Recreate base references.
        /// </summary>
        /// <param name="container">Container instance.</param>
        /// <param name="errorString">Error text.</param>
        public static bool RecreateReferences(Container container, out string errorString)
        {
            new List<OutputMessageCollection> { container.Application.OutputMessages, container.Deploy.OutputMessages }
                .ForEach(item => item.Clear());

            if (!InfoUtils.IsRequiredFieldsFilled(container, out errorString) ||
                !CreateManifestFile(container, out errorString) || !CreateDeployFile(container, out errorString))
            {
                return false;
            }

            if (container.Deploy.MapFileExtensions)
            {
                FlowUtils.AddDeployExtention(container.FullPath);
            }

            return true;
        }

        private static bool CreateDeployFile(Container container, out string errorString)
        {
            DeployManifest deploy = container.Deploy;

            // Set deploy entry point identity
            SetDeployEntrypointIdentity(container);

            // Set AssemblyReferences
            AddManifestReference(container);

            // Set global important settings
            FlowUtils.SetGlobals(container.Deploy, container);

            if (!InfoUtils.IsValidManifest(deploy, out errorString))
            {
                return false;
            }

            // Writing to file
            ManifestWriter.WriteManifest(deploy, deploy.SourcePath, FlowUtils.GetTargetFramework(container));

            ProcessAfterSave(deploy, container.Application.TargetFrameworkVersion);
            return true;
        }

        private static void ProcessAfterSave(Manifest manifest, string targetFrameworkVersion)
        {
            if (targetFrameworkVersion == "v4.5")
            {
                // TODO Bugfix ! But if your NETFX have a fix it'll do nothing.
                // https://connect.microsoft.com/VisualStudio/feedback/details/754487/mage-exe-hashes-with-sha1-but-maintains-to-hash-with-sha256
                /*
                    "Application manifest has either a different computed hash than the one specified or no hash specified at all."

                    Simply change
                        <dsig:DigestMethod Algorithm="http://www.w3.org/2000/09/xmldsig#sha256" />
                    to
                        <dsig:DigestMethod Algorithm="http://www.w3.org/2000/09/xmldsig#sha1" />
                    in the affected manifest files.
                */

                string text = File.ReadAllText(manifest.SourcePath);
                text = text.Replace(
                    "http://www.w3.org/2000/09/xmldsig#sha1",
                    "http://www.w3.org/2000/09/xmldsig#sha256");
                File.WriteAllText(manifest.SourcePath, text);
            }
        }

        private static bool CreateManifestFile(Container container, out string errorString)
        {
            // Add other file references
            ReferenceUtils.AddAssemblyReferences(container);

            // Set TrustInfo property
            SetApplicationTrustInfo(container);

            // Set endpoint and assemblyIdentity (by the way its a similar things)
            SetApplicationEndpointIdentity(container);

            // Set global important settings
            FlowUtils.SetGlobals(container.Application, container);

            if (!InfoUtils.IsValidManifest(container.Application, out errorString))
            {
                return false;
            }

            // Writing to file
            ManifestWriter.WriteManifest(
                container.Application,
                container.Application.SourcePath,
                FlowUtils.GetTargetFramework(container));

            ProcessAfterSave(container.Application, container.Application.TargetFrameworkVersion);

            return true;
        }

        private static void AddManifestReference(Container container)
        {
            var deploy = container.Deploy;

            deploy.AssemblyReferences.Clear();
            deploy.FileReferences.Clear();

            var manifestReference = new AssemblyReference(container.Application.SourcePath)
            {
                ReferenceType = AssemblyReferenceType.ClickOnceManifest
            };

            deploy.AssemblyReferences.Add(manifestReference);
        }

        private static void SetApplicationTrustInfo(Container container)
        {
            var application = container.Application;

            // TODO FullTrusted for now
            application.TrustInfo = new TrustInfo() { IsFullTrust = true };
        }

        private static void SetApplicationEndpointIdentity(Container container)
        {
            var application = container.Application;

            for (int i = 0; i < application.AssemblyReferences.Count; i++)
            {
                var refrence = application.AssemblyReferences[i];
                if (refrence.SourcePath == container.EntrypointPath)
                {
                    application.EntryPoint = refrence;
                    application.AssemblyIdentity = new AssemblyIdentity(refrence.AssemblyIdentity);
                    break;
                }
            }

            application.AssemblyIdentity.Version = container.Version;
            application.AssemblyIdentity.Name = container.ApplicationName;

            /*
             In the case of assembly damaged or assembly reference not exists in directory. Be insure about accessibility of all assemblies.

             ERROR DETAILS
	            Following errors were detected during this operation.
	            * [14.03.2017 23:25:16] System.Deployment.Application.InvalidDeploymentException (RefDefValidation)
		            - Reference in the manifest does not match the identity of the downloaded assembly ClickOnceGen.exe.
		            - Source: System.Deployment
		            - Stack trace:
			            at System.Deployment.Application.DownloadManager.ProcessDownloadedFile(Object sender, DownloadEventArgs e)
			            at System.Deployment.Application.FileDownloader.DownloadModifiedEventHandler.Invoke(Object sender, DownloadEventArgs e)
			            at System.Deployment.Application.FileDownloader.OnModified()
			            at System.Deployment.Application.SystemNetDownloader.DownloadSingleFile(DownloadQueueItem next)
			            at System.Deployment.Application.SystemNetDownloader.DownloadAllFiles()
             */
        }

        private static void SetDeployEntrypointIdentity(Container container)
        {
            var manifestPath = container.Application.SourcePath;
            container.Deploy.AssemblyIdentity.Name = container.ApplicationName;

            container.Deploy.AssemblyIdentity.Version = container.Version;

            var manifestReference = new AssemblyReference(manifestPath)
            {
                AssemblyIdentity = new AssemblyIdentity(container.Application.AssemblyIdentity)
            };
            container.Deploy.AssemblyReferences.Add(manifestReference);
            container.Deploy.EntryPoint = manifestReference;
        }
    }
}