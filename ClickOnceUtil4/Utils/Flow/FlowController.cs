using System.Collections.Generic;
using System.Linq;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.Utils.Flow.FlowOperations;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// User action flow controller.
    /// </summary>
    public class FlowController
    {
        private readonly IDictionary<UserActions, FlowBase> _flows = new FlowBase[]
        {
            new NewFlow(),
            new RemoveFlow(),
            new UpdateFlow(),
            new ResigningFlow()
        }.ToDictionary(item => item.UserAction, item => item);

        /// <summary>
        /// Get available action for folder. 
        /// </summary>
        /// <param name="folderInfo"><see cref="ClickOnceFolderInfo"/> object reference.</param>
        /// <returns>User actions.</returns>
        public IEnumerable<UserActions> GetActions(ClickOnceFolderInfo folderInfo)
        {
            var folderType = folderInfo.FolderType;
            var fullPath = folderInfo.FullPath;

            return _flows.Values.Where(flow => flow.IsFlowApplicable(folderType, fullPath)).Select(flow => flow.UserAction).ToArray();
        }

        /// <summary>
        /// Get flow for action.
        /// </summary>
        /// <param name="userAction">User action.</param>
        /// <returns><see cref="FlowBase"/> object.</returns>
        public FlowBase this[UserActions userAction] => _flows[userAction];
    }
}