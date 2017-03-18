using System.Collections.Generic;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

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
    }
}