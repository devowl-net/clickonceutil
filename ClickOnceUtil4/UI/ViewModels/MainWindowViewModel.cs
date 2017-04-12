using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils;
using ClickOnceUtil4UI.Utils.Flow;
using ClickOnceUtil4UI.Utils.Prism;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Win32;

using BuildDeployManifest = Microsoft.Build.Tasks.Deployment.ManifestUtilities.DeployManifest;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private readonly FlowsContainer _flowsContainer = new FlowsContainer();

        private ClickOnceFolderInfo _selectedFolder;

        private ManifestEditorViewModel<BuildDeployManifest> _deployManifest;

        private ManifestEditorViewModel<ApplicationManifest> _applicationManifest;

        private UserActions _selectedAction = UserActions.None;

        private string _selectedEntrypoint = string.Empty;

        private string _version;

        private string _applicationName;

        private string _selectedCetificatePath;

        private string _timestampUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            MenuViewModel = new MenuViewModel(this);
            ChooseCommand = new DelegateCommand(ChooseHandler);
            BuildCommand = new DelegateCommand(BuildHandler);
            CleanCacheCommand = new DelegateCommand(CleanCacheHandler);
            ChooseCertificateCommand = new DelegateCommand(ChooseCertificateHandler);
            BrowseCommand = new DelegateCommand(BrowseHandler, CanBrowse);

            /*
            var newFolder = new ClickOnceFolderInfo(@"C:\IISRoot\DELME");
            newFolder.Update(true);
            SelectedFolder = newFolder;

            SelectedCetificatePath = @"C:\IISRoot\Certificate_Password_123456.pfx";
            */
        }

        /// <summary>
        /// Menu view model.
        /// </summary>
        public MenuViewModel MenuViewModel { get; set; }

        /// <summary>
        /// Timestamp server URL.
        /// </summary>
        public string TimestampUrl
        {
            get
            {
                return _timestampUrl;
            }

            set
            {
                _timestampUrl = value;
                RaisePropertyChanged(() => TimestampUrl);
            }
        }

        /// <summary>
        /// Create and use temporary certificate.
        /// </summary>
        public bool IsTemporaryCertificate { get; set; }

        /// <summary>
        /// Path to selected certificate file.
        /// </summary>
        public string SelectedCetificatePath
        {
            get
            {
                return _selectedCetificatePath;
            }

            set
            {
                _selectedCetificatePath = value;
                RaisePropertyChanged(() => SelectedCetificatePath);
            }
        }

        /// <summary>
        /// Choose folder button command.
        /// </summary>
        public DelegateCommand ChooseCommand { get; private set; }

        /// <summary>
        /// Build button command.
        /// </summary>
        public DelegateCommand BuildCommand { get; private set; }

        /// <summary>
        /// Clean cache button command.
        /// </summary>
        public DelegateCommand CleanCacheCommand { get; private set; }

        /// <summary>
        /// Browse button command.
        /// </summary>
        public DelegateCommand BrowseCommand { get; private set; }

        /// <summary>
        /// Choose certificate file command.
        /// </summary>
        public DelegateCommand ChooseCertificateCommand { get; private set; }

        /// <summary>
        /// Folder source path.
        /// </summary>
        public ClickOnceFolderInfo SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }

            private set
            {
                _selectedFolder = value;
                RaisePropertyChanged(() => SelectedFolder);
                FolderUpdated(value);
                BrowseCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Available actions for selected folder.
        /// </summary>
        public ObservableCollection<UserActions> AvaliableActions { get; set; } =
            new ObservableCollection<UserActions>();

        /// <summary>
        /// User selected action.
        /// </summary>
        public UserActions SelectedAction
        {
            get
            {
                return _selectedAction;
            }

            set
            {
                _selectedAction = value;
                RaisePropertyChanged(() => SelectedAction);
                SelectedActionChanges(value);
            }
        }

        /// <summary>
        /// Deploy manifest view model.
        /// </summary>
        public ManifestEditorViewModel<DeployManifest> DeployManifest
        {
            get
            {
                return _deployManifest;
            }

            set
            {
                _deployManifest = value;
                RaisePropertyChanged(() => DeployManifest);
            }
        }

        /// <summary>
        /// Application manifest view model.
        /// </summary>
        public ManifestEditorViewModel<ApplicationManifest> ApplicationManifest
        {
            get
            {
                return _applicationManifest;
            }

            set
            {
                _applicationManifest = value;
                RaisePropertyChanged(() => ApplicationManifest);
            }
        }

        /// <summary>
        /// ClickOnce directory executable file names.
        /// </summary> 
        public ObservableCollection<string> ApplicationEntryPoints { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Certificate file password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ClickOnce application version
        /// </summary>
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged(() => Version);
            }
        }

        /// <summary>
        /// Selected entry point value.
        /// </summary>
        public string SelectedEntrypoint
        {
            get
            {
                return _selectedEntrypoint;
            }
            set
            {
                _selectedEntrypoint = value;
                RaisePropertyChanged(() => SelectedEntrypoint);
                SelectedEntrypointChanged(_selectedEntrypoint);
            }
        }

        /// <summary>
        /// Application name. 
        /// </summary>
        /// <remarks>The name shouldn't be like executable file name.</remarks>
        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }

            set
            {
                _applicationName = value;
                RaisePropertyChanged(() => ApplicationName);
            }
        }

        private bool CanBrowse(object obj)
        {
            return SelectedFolder != null && SelectedFolder.HasDeployManifest &&
                   !string.IsNullOrEmpty(SelectedFolder.DeployManifest.DeploymentUrl);
        }

        private void BrowseHandler(object obj)
        {
            if (CanBrowse(null))
            {
                var deployUrl = SelectedFolder.DeployManifest.DeploymentUrl.Trim();
                if (deployUrl.StartsWith(Uri.UriSchemeHttp))
                {
                    Process.Start("IEXPLORE.EXE", deployUrl);
                }
                else if (deployUrl.StartsWith("\\\\"))
                {
                    Process.Start(deployUrl);
                }
                else
                {
                    MessageBox.Show("Unknown deploy address");
                }
            }
            else
            {
                BrowseCommand.RaiseCanExecuteChanged();
            }
        }

        private void ChooseCertificateHandler(object obj)
        {
            // https://msdn.microsoft.com/en-us/library/che5h906.aspx
            var fileDialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                Filter = "Certificate Files|*.pfx",
                Multiselect = false,
                InitialDirectory = Environment.CurrentDirectory,
                Title = "Please choose certificate file",
                CheckPathExists = true
            };

            if (fileDialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                SelectedCetificatePath = fileDialog.FileName;
            }
        }

        private void SelectedEntrypointChanged(string selectedEntrypoint)
        {
            ApplicationName = selectedEntrypoint;
            if (DeployManifest != null && !string.IsNullOrEmpty(SelectedEntrypoint))
            {
                var deployFileName =
                    $"{Path.GetFileNameWithoutExtension(SelectedEntrypoint)}.{Constants.ApplicationExtension}";

                // Set DeployUrl
                var deploymentUrl = FlowUtils.GetDeployUrl(SelectedFolder.FullPath, deployFileName);

                var propertyField =
                    DeployManifest.Properties.First(p => p.PropertyName == nameof(BuildDeployManifest.DeploymentUrl));

                propertyField.StringValue = deploymentUrl;

                // Set SourcePath
                var root = SelectedFolder.FullPath;
                propertyField =
                    DeployManifest.Properties.First(p => p.PropertyName == nameof(BuildDeployManifest.SourcePath));
                propertyField.StringValue = Path.Combine(root, deployFileName);
            }
        }

        private void FolderUpdated(ClickOnceFolderInfo value)
        {
            AvaliableActions.Clear();

            if (value != null)
            {
                foreach (var action in _flowsContainer.GetActions(value))
                {
                    AvaliableActions.Add(action);
                }
            }

            SelectedAction = AvaliableActions.FirstOrDefault();
        }

        private X509Certificate2 GetCertificate()
        {
            if (IsTemporaryCertificate || string.IsNullOrEmpty(SelectedCetificatePath))
            {
                return null;
            }

            if (string.IsNullOrEmpty(Password))
            {
                return new X509Certificate2(X509Certificate.CreateFromSignedFile(SelectedCetificatePath));
            }
            var secureString = new SecureString();
            foreach (var chr in Password)
            {
                secureString.AppendChar(chr);
            }
            
            return new X509Certificate2(
                SelectedCetificatePath,
                secureString,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
        }

        private void BuildHandler(object obj)
        {
            if (SelectedAction == UserActions.Resigning)
            {
                if (!IsTemporaryCertificate && string.IsNullOrEmpty(SelectedCetificatePath))
                {
                    MessageBox.Show(
                        "No certificates was selected",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!string.IsNullOrEmpty(TimestampUrl))
                {
                    try
                    {
                        var uri = new Uri(TimestampUrl);
                        if (string.IsNullOrEmpty(uri.Host))
                        {
                            throw new Exception("No host name in Uri");
                        }
                    }
                    catch (Exception exception)
                    {
                        string text = $"Unable to create Uri from timestamp {TimestampUrl}. Error text below:{Environment.NewLine + Environment.NewLine}{exception.Message}";
                        MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            

            X509Certificate2 certificate = null;

            if (SelectedAction == UserActions.Resigning)
            {
                try
                {
                    certificate = GetCertificate();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        exception.Message,
                        "Certificate error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }

            var container = new Container
            {
                TimestampUrl = TimestampUrl,
                FullPath = SelectedFolder.FullPath,
                Application = ApplicationManifest?.Manifest ?? SelectedFolder.ApplicationManifest,
                Deploy = DeployManifest?.Manifest ?? SelectedFolder.DeployManifest,
                Certificate = certificate,
                Version = _version,
                EntrypointPath =
                    !string.IsNullOrEmpty(SelectedEntrypoint)
                        ? Path.Combine(SelectedFolder.FullPath, SelectedEntrypoint)
                        : null
            };

            container.ApplicationName = !string.IsNullOrEmpty(ApplicationName)
                ? ApplicationName
                : Path.GetFileName(container.EntrypointPath);

            var buildInfo = _flowsContainer[SelectedAction].GetBuildInformation(container).ToArray();
            if (buildInfo.Any())
            {
                var buildModel = new BuildInfoViewModel(buildInfo);
                var buildView = new BuildInfoView(buildModel) { Owner = Application.Current.MainWindow };
                if (!buildView.ShowDialog().GetValueOrDefault())
                {
                    return;
                }
            }

            string errorString;
            if (!_flowsContainer[SelectedAction].Execute(container, out errorString))
            {
                MessageBox.Show(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(
                    "Operation completed successfully!",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                _selectedFolder.Update(true);
                FolderUpdated(_selectedFolder);
            }

            BrowseCommand.RaiseCanExecuteChanged();
        }

        private void CleanCacheHandler(object obj)
        {
            var appsFolder = $@"C:\Users\{Environment.UserName}\AppData\Local\Apps\2.0";
            var buildInfo = new[]
            {
                new InfoData(
                    "Clean cache",
                    $@"You are going to clean local applications cache. Be sure that no deployed programs running now. Cache folder location: ""{
                        appsFolder}"". By the way cleaning can be done manually:{Environment.NewLine
                        }1. By cmd.exe command: ""rundll32 dfshim CleanOnlineAppCache""] {Environment.NewLine
                        }2. Or just remove [{appsFolder}] folder content.")
            };

            var buildModel = new BuildInfoViewModel(buildInfo);
            var buildView = new BuildInfoView(buildModel) { Owner = Application.Current.MainWindow };
            if (buildView.ShowDialog().GetValueOrDefault())
            {
                string errorString, messageText;
                if (FlowUtils.CleanCache(out errorString))
                {
                    messageText = "Operation completed!";
                }
                else
                {
                    messageText = $"Unable complete cleaning cause of error:{Environment.NewLine}{errorString}";
                }

                MessageBox.Show(messageText, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChooseHandler(object obj)
        {
            var dataContext = new ChooseFolderDialogViewModel(SelectedFolder?.FullPath);
            var dialog = new ChooseFolderDialog(dataContext) { Owner = Application.Current.MainWindow };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SelectedFolder = dataContext.SelectedFolder;
            }
        }

        private void InitEntrypoints(UserActions action)
        {
            ApplicationEntryPoints.Clear();
            switch (action)
            {
                case UserActions.New:
                    var rootDirectory = new DirectoryInfo(SelectedFolder.FullPath);
                    var entrypoints =
                        rootDirectory.GetFiles("*.exe")
                            .Union(rootDirectory.GetFiles($"*.{Constants.DeployFileExtension}"))
                            .ToArray();

                    foreach (var entrypointFile in entrypoints)
                    {
                        ApplicationEntryPoints.Add(ReferenceUtils.GetNormalFilePath(entrypointFile.Name));
                    }

                    SelectedEntrypoint = entrypoints.Select(entry => entry.Name).FirstOrDefault();
                    break;
                case UserActions.Update:
                    var entrypoint = SelectedFolder.ApplicationManifest?.EntryPoint;
                    if (entrypoint != null)
                    {
                        ApplicationEntryPoints.Add(entrypoint.TargetPath);
                        SelectedEntrypoint = entrypoint.TargetPath;
                    }
                    break;
            }
        }

        private void SelectedActionChanges(UserActions action)
        {
            InitEntrypoints(action);
            if (action == UserActions.New || action == UserActions.Update)
            {
                var deploy = SelectedFolder.DeployManifest ??
                             FlowUtils.CreateDeployManifest(SelectedFolder.FullPath, SelectedEntrypoint);
                var application = SelectedFolder.ApplicationManifest ??
                                  FlowUtils.CreateApplicationManifest(SelectedFolder.FullPath, SelectedEntrypoint);

                Version = FlowUtils.ReadApplicationVersion(deploy) ?? Constants.DefaultVersion;
                ApplicationName = FlowUtils.ReadApplicationName(application);

                DeployManifest = new ManifestEditorViewModel<DeployManifest>(deploy);
                ApplicationManifest = new ManifestEditorViewModel<ApplicationManifest>(application);
            }
            else
            {
                DeployManifest = null;
                ApplicationManifest = null;
            }
        }
    }
}