using System;
using System.Collections.Generic;
using System.Text;

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
        /// <param name="container">Objects container.</param>
        /// <param name="errorString">Contains error string.</param>
        /// <returns>Executing result.</returns>
        public abstract bool Execute(Container container, out string errorString);

        /// <summary>
        /// Get information text for user action.
        /// </summary>
        /// <param name="container">Objects container.</param>
        /// <returns>Information data objects.</returns>
        public abstract IEnumerable<InfoData> GetBuildInformation(Container container);

        /// <summary>
        /// Check filling of required for generation fields.
        /// </summary>
        /// <param name="container">Objects container.</param>
        /// <param name="errorString">Contains error string.</param>
        /// <returns></returns>
        protected bool IsRequiredFieldsFilled(Container container, out string errorString)
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
        protected bool IsValidManifest(Manifest manifest, out string errorString)
        {
            errorString = null;
            manifest.Validate();

            if (manifest.OutputMessages.ErrorCount > 0)
            {
                errorString =
                    $"{manifest.GetType().Name} errors:{Environment.NewLine + Environment.NewLine}{ReadOutputMessages(manifest.OutputMessages)}";
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