using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.Utils;
using ClickOnceUtil4UI.Utils.Prism;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.UI.Models
{
    /// <summary>
    /// ClickOnce information about folder.
    /// </summary>
    public class ClickOnceFolderInfo : NotificationObject
    {
        private FolderTypes _folderType = FolderTypes.CommonFolder;

        private string _errorDescription;

        private string _manifestFileName;

        private string _applicationFileName;

        private ApplicationManifest _applicationManifest;

        private DeployManifest _deployManifest;

        private string _applicationManifestError;

        private string _deployManifestError;

        /// <summary>
        /// Constructor for  <see cref="ClickOnceFolderInfo"/>.
        /// </summary>
        public ClickOnceFolderInfo(string path)
        {
            FullPath = path;
            Name = Path.GetFileName(path);
            Update();
        }

        /// <summary>
        /// Folder Type value.
        /// </summary>
        public FolderTypes FolderType
        {
            get
            {
                return _folderType;
            }
            set
            {
                _folderType = value;
                RaisePropertyChanged(() => FolderType);
            }
        }

        /// <summary>
        /// Folder Name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Full folder path.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Manifest file name.
        /// </summary>
        public string ManifestFileName
        {
            get
            {
                return _manifestFileName;
            }

            private set
            {
                _manifestFileName = value;
                RaisePropertyChanged(() => ManifestFileName);
            }
        }

        /// <summary>
        /// Application file name.
        /// </summary>
        public string ApplicationFileName
        {
            get
            {
                return _applicationFileName;
            }

            private set
            {
                _applicationFileName = value;
                RaisePropertyChanged(() => ApplicationFileName);
            }
        }

        /// <summary>
        /// Error description.
        /// </summary>
        public string ErrorDescription
        {
            get
            {
                return _errorDescription;
            }

            private set
            {
                _errorDescription = value;
                RaisePropertyChanged(() => ErrorDescription);
            }
        }

        /// <summary>
        /// ClickOnce .manifest file.
        /// </summary>
        public ApplicationManifest ApplicationManifest
        {
            get
            {
                return _applicationManifest;
            }

            private set
            {
                _applicationManifest = value;
                RaisePropertyChanged(() => ApplicationManifest);
            }
        }

        /// <summary>
        /// ClickOnce .application read error text.
        /// </summary>
        public string DeployManifestError
        {
            get
            {
                return _deployManifestError;
            }

            set
            {
                _deployManifestError = value;
                RaisePropertyChanged(() => DeployManifestError);
            }
        }

        /// <summary>
        /// ClickOnce .manifest read error text.
        /// </summary>
        public string ApplicationManifestError
        {
            get
            {
                return _applicationManifestError;
            }

            set
            {
                _applicationManifestError = value;
                RaisePropertyChanged(() => ApplicationManifestError);
            }
        }

        /// <summary>
        /// ClickOnce .application file.
        /// </summary>
        public DeployManifest DeployManifest
        {
            get
            {
                return _deployManifest;
            }

            private set
            {
                _deployManifest = value;
                RaisePropertyChanged(() => DeployManifest);
            }
        }

        /// <summary>
        /// Has application manifest.
        /// </summary>
        public bool HasApplicationManifest => ApplicationManifest != null;

        /// <summary>
        /// Has deploy manifest.
        /// </summary>
        public bool HasDeployManifest => DeployManifest != null;

        /// <summary>
        /// Update folder information.
        /// </summary>
        /// <param name="synchronously">Synchronously update or not.</param>
        public void Update(bool synchronously = false)
        {
            if (PathUtils.IsIgnoredPath(FullPath))
            {
                return;
            }

            try
            {
                if (synchronously)
                {
                    InternalUpdate();
                }
                else
                {
                    Task.Factory.StartNew(InternalUpdate);
                }
            }
            catch (Exception exception)
            {
                FolderType = FolderTypes.HaveProblems;
                ErrorDescription = exception.Message;
            }
        }

        private static string GetFilesAmountError(string fileExtension, int amount)
        {
            return $"Folder contains more then one .{fileExtension} files. Amount is {amount}";
        }

        private void InternalUpdate()
        {
            ApplicationManifest = null;
            DeployManifest = null;

            if (!PathUtils.CheckFolderReadPermissions(FullPath))
            {
                FolderType = FolderTypes.HaveProblems;
                ErrorDescription = "No folder read permissions";
                return;
            }

            string deployManifestFile, manifestFile, errorMessage;
            int deployManifestFileCount, manifestFileCount;

            // Reads .application
            if (ClickOnceFolderInfoUtils.TryGetSingleFile(
                FullPath,
                Constants.ManifestExtension,
                out deployManifestFileCount,
                out deployManifestFile))
            {
                ApplicationFileName = Path.GetFileName(deployManifestFile);
                ApplicationManifest appManifest;

                if (
                    !ClickOnceFolderInfoUtils.TryReadClickOnceFile(
                        deployManifestFile,
                        out appManifest,
                        out errorMessage))
                {
                    ApplicationManifestError = errorMessage;
                    FolderType = FolderTypes.UnknownClickOnceApplication;
                    return;
                }

                ApplicationManifest = appManifest;
            }
            else
            {
                if (deployManifestFileCount > 1)
                {
                    ApplicationManifestError = GetFilesAmountError(Constants.ManifestExtension, deployManifestFileCount);
                }
            }

            // Reads .manifest
            if (ClickOnceFolderInfoUtils.TryGetSingleFile(
                FullPath,
                Constants.ApplicationExtension,
                out manifestFileCount,
                out manifestFile))
            {
                ManifestFileName = Path.GetFileName(manifestFile);
                DeployManifest deployManifest;
                if (!ClickOnceFolderInfoUtils.TryReadClickOnceFile(manifestFile, out deployManifest, out errorMessage))
                {
                    ApplicationManifestError = errorMessage;
                    FolderType = FolderTypes.UnknownClickOnceApplication;
                    return;
                }

                DeployManifest = deployManifest;
            }
            else
            {
                if (manifestFileCount > 1)
                {
                    DeployManifestError = GetFilesAmountError(Constants.ApplicationExtension, manifestFileCount);
                }
            }

            if (HasApplicationManifest && HasDeployManifest)
            {
                FolderType = FolderTypes.ClickOnceApplication;
            }
            else
            {
                // deciding about ClickOnce application possibilities 
                if (ClickOnceFolderInfoUtils.IsFolderCanBeClickOnceApplication(FullPath, out errorMessage))
                {
                    FolderType = FolderTypes.CanBeAnApplication;
                }
                else
                {
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ApplicationManifestError = errorMessage;
                        FolderType = FolderTypes.UnknownClickOnceApplication;
                    }
                }
            }
        }
    }
}