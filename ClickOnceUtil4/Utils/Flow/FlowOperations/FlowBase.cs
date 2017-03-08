using System.Security.Cryptography.X509Certificates;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow.FlowOperations
{
    /// <summary>
    /// Base class for flows.
    /// </summary>
    public abstract class FlowBase
    {
        /// <summary>
        /// Constructor for <see cref="FlowBase"/>.
        /// </summary>
        protected FlowBase(UserActions userAction)
        {
            UserAction = userAction;
        }

        /// <summary>
        /// Flow Action.
        /// </summary>
        public UserActions UserAction { get; }

        /// <summary>
        /// Check path for possible of presented <see cref="UserAction"/>.
        /// </summary>
        /// <param name="folderType">Computed folder type.</param>
        /// <param name="fullPath">Full path to folder.</param>
        /// <returns>Is it flow applicable for source path.</returns>
        public abstract bool IsFlowApplicable(FolderTypes folderType, string fullPath);

        /// <summary>
        /// Execute flow.
        /// </summary>
        /// <param name="application">Filled <see cref="ApplicationManifest"/> instance.</param>
        /// <param name="deploy">Filled <see cref="DeployManifest"/> instance.</param>
        /// <param name="certificate">Required certificate instance.</param>
        /// <param name="errorString">Contains error string.</param>
        /// <returns>Executing result.</returns>
        public abstract bool Execute(ApplicationManifest application, DeployManifest deploy, X509Certificate2 certificate, out string errorString);
    }
}
