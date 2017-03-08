using System.Security.Cryptography.X509Certificates;

using ClickOnceUtil4UI.Clickonce;

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
        public override bool Execute(ApplicationManifest application, DeployManifest deploy, X509Certificate2 certificate, out string errorString)
        {
            throw new System.NotImplementedException();
        }
    }
}