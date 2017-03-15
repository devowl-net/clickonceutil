using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow.FlowOperations
{
    /// <summary>
    /// Flow for remove action.
    /// </summary>
    internal class RemoveFlow : FlowBase
    {
        public RemoveFlow() : base(UserActions.Remove)
        {
        }

        public override bool IsFlowApplicable(FolderTypes folderType, string fullPath)
        {
            return folderType == FolderTypes.ClickOnceApplication ||
                   Directory.GetFiles(fullPath, $"*. {Constants.DeployFileExtension}").Any();
        }

        /// <inheritdoc/>
        public override bool Execute(Container container, out string errorString)
        {
            string fullPath = container.FullPath;
            errorString = null;
            try
            {
                if (string.IsNullOrEmpty(fullPath))
                {
                    errorString = $"{nameof(DeployManifest.SourcePath)} value is not path.";
                    return false;
                }

                FlowUtils.RemoveDeployExtention(fullPath);
                var currentDirectory = new DirectoryInfo(fullPath);
                var manifestFiles =
                    currentDirectory.GetFiles($"*.{Constants.ApplicationExtension}")
                        .Union(currentDirectory.GetFiles($"*.{Constants.ManifestExtension}")).ToArray();

                foreach (var manifestFile in manifestFiles)
                {
                    File.Delete(manifestFile.FullName);    
                }

                return true;
            }
            catch (Exception exception)
            {
                errorString = exception.Message;
                return false;
            }
        }
    }
}