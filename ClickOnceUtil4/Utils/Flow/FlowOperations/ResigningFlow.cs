using System.Security.Cryptography.X509Certificates;

using ClickOnceUtil4UI.Clickonce;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow.FlowOperations
{
    /// <summary>
    /// Resigning flow scenario.
    /// </summary>
    internal class ResigningFlow : FlowBase
    {
        /// <summary>
        /// Constructor for <see cref="ResigningFlow"/>.
        /// </summary>
        public ResigningFlow() : base(UserActions.Resigning)
        {
        }

        /// <inheritdoc/>
        public override bool IsFlowApplicable(FolderTypes folderType, string fullPath)
        {
            return folderType == FolderTypes.ClickOnceApplication;
        }

        /// <inheritdoc/>
        public override bool Execute(
            ApplicationManifest application,
            DeployManifest deploy,
            X509Certificate2 certificate,
            out string errorString)
        {
            errorString = null;
            certificate = certificate ?? CertificateUtils.GenerateSelfSignedCertificate();

            if (application != null)
            {
                var path = application.SourcePath;
                SecurityUtilities.SignFile(certificate, null, path);
            }

            if (deploy != null)
            {
                var path = deploy.SourcePath;
                SecurityUtilities.SignFile(certificate, null, path);
            }

            return true;
        }
    }
}