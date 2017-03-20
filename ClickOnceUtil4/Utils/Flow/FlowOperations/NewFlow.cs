using System.Collections.Generic;
using System.IO;
using System.Linq;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow.FlowOperations
{
    /// <summary>
    /// New flow scenario.
    /// </summary>
    internal class NewFlow : FlowBase
    {
        /// <summary>
        /// Constructor for <see cref="NewFlow"/>.
        /// </summary>
        public NewFlow() : base(UserActions.New)
        {
        }

        /// <inheritdoc/>
        public override bool IsFlowApplicable(FolderTypes folderType, string fullPath)
        {
            return folderType == FolderTypes.CanBeAnApplication;
        }

        /// <inheritdoc/>
        public override bool Execute(Container container, out string errorString)
        {
            new List<OutputMessageCollection> { container.Application.OutputMessages, container.Deploy.OutputMessages }
                .ForEach(item => item.Clear());

            if (!InfoUtils.IsRequiredFieldsFilled(container, out errorString) || !CreateManifestFile(container, out errorString) ||
                !CreateDeployFile(container, out errorString))
            {
                return false;
            }

            if (container.Deploy.MapFileExtensions)
            {
                FlowUtils.AddDeployExtention(container.FullPath);
            }

            return true;
        }

        /// <inheritdoc/>
        public override IEnumerable<InfoData> GetBuildInformation(Container container)
        {
            return
                InfoUtils.GetApplicationInfoData(container.Application)
                    .Union(InfoUtils.GetDeployInfoData(container.Deploy));
        }
        private bool CreateDeployFile(Container container, out string errorString)
        {
            DeployManifest deploy = container.Deploy;

            // Set deploy entry point identity
            SetDeployEntrypointIdentity(container);

            // Set AssemblyReferences
            AddManifestReference(container);

            // Set global important settings
            FlowUtils.SetGlobals(container.Deploy);

            if (!InfoUtils.IsValidManifest(deploy, out errorString))
            {
                return false;
            }

            // Writing to file
            ManifestWriter.WriteManifest(deploy, deploy.SourcePath, Constants.DefaultFramework);

            return true;
        }

        private bool CreateManifestFile(Container container, out string errorString)
        {
            // Add other file references
            ReferenceUtils.AddReferences(container.Application, container.FullPath);

            // Set TrustInfo property
            SetApplicationTrustInfo(container);

            // Set endpoint and assemblyIdentity (by the way its a similar things)
            SetApplicationEndpointIdentity(container);

            // Set global important settings
            FlowUtils.SetGlobals(container.Application);

            if (!InfoUtils.IsValidManifest(container.Application, out errorString))
            {
                return false;
            }

            // Writing to file
            ManifestWriter.WriteManifest(
                container.Application,
                container.Application.SourcePath,
                Constants.DefaultFramework);

            return true;
        }

        private void AddManifestReference(Container container)
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

        private void SetApplicationTrustInfo(Container container)
        {
            var application = container.Application;

            // TODO FullTrusted for now
            application.TrustInfo = new TrustInfo() { IsFullTrust = true };
        }

        private void SetApplicationEndpointIdentity(Container container)
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

        private void SetDeployEntrypointIdentity(Container container)
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