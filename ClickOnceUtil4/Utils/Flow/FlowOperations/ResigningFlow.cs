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
        public override bool Execute(Container container, out string errorString)
        {
            errorString = null;
            
            FlowUtils.SignFile(container.Application, container.Certificate);
            FlowUtils.SignFile(container.Deploy, container.Certificate);
            return true;
        }
    }
}