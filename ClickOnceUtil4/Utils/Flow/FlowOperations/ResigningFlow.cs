using System;
using System.Collections.Generic;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

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

        /// <inheritdoc/>
        public override IEnumerable<InfoData> GetBuildInformation(Container container)
        {
            string description = container.Certificate == null
                ? "Certificate file will be generated automatically. Publisher name: \"CN = TempCA\""
                : $"Certificate date:{Environment.NewLine}{container.Certificate}";

            yield return new InfoData(nameof(container.Certificate), description);
        }
    }
}