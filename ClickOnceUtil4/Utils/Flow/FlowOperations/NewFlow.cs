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
            return UpdateManifestUtils.RecreateReferences(container, out errorString);
        }

        /// <inheritdoc/>
        public override IEnumerable<InfoData> GetBuildInformation(Container container)
        {
            return InfoUtils.GetFullInfoData(container);
        }
    }
}