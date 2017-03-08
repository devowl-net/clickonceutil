using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Windows;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Flow;
using ClickOnceUtil4UI.Utils.Prism;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private readonly FlowController _flowController = new FlowController();

        private ClickOnceFolderInfo _selectedFolder;

        private ManifestEditorViewModel<DeployManifest> _deployManifest;

        private ManifestEditorViewModel<ApplicationManifest> _applicationManifest;

        private UserActions _selectedAction = UserActions.None;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindowViewModel()
        {
            ChooseCommand = new DelegateCommand(ChooseHandler);
            BuildCommand = new DelegateCommand(BuildHandler);

            // TODO DELETE
            var newFolder = new ClickOnceFolderInfo(@"C:\_WpfApplication4_1_0_0_52");
            newFolder.Update(true);
            SelectedFolder = newFolder;
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
                if (value != null)
                {
                    AvaliableActions.Clear();
                    foreach (var action in _flowController.GetActions(value))
                    {
                        AvaliableActions.Add(action);
                    }
                }
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

        private void BuildHandler(object obj)
        {
            string errorString;
            if (
                !_flowController[SelectedAction].Execute(
                    SelectedFolder.ApplicationManifest,
                    SelectedFolder.DeployManifest,
                    null,
                    out errorString))
            {
                MessageBox.Show(errorString, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("Operation completed successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Deploy (.application) http://take.ms/6vydt
            // Application (.manifest) http://take.ms/v099L
            
            // var deployManifest = DeployManifest.Manifest;
            // var applicationManifest = ApplicationManifest.Manifest;
            // 
            // var deployOutputMessages = deployManifest.OutputMessages;
            // var applicationOutputMessages = applicationManifest.OutputMessages;
            // 
            // DeployManifest.Manifest.Validate();
            // ApplicationManifest.Manifest.Validate();
            // 
            // if (deployOutputMessages.ErrorCount > 0 && applicationOutputMessages.ErrorCount > 0)
            // {
            //     var messageText =
            //         $"{ReadOutputMessages(deployOutputMessages)}{Environment.NewLine}{ReadOutputMessages(applicationOutputMessages)}";
            // 
            //     MessageBox.Show(messageText);
            //     return;
            // }
        }

        private StringBuilder ReadOutputMessages(OutputMessageCollection outputMessages)
        {
            var buffer = new StringBuilder();
            int counter = 1;

            foreach (var outputMessage in outputMessages)
            {
                buffer.AppendFormat($"{counter}) {outputMessage}");
                buffer.AppendLine();
                counter++;
            }

            return buffer;
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

        private DeployManifest CreateDeployManifest()
        {
            Version clickOnceUtilVersion =
                new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

            Version manikerVersion = new Version(clickOnceUtilVersion.Major, clickOnceUtilVersion.Minor);
            return new DeployManifest((new FrameworkName(".NETFramework", manikerVersion, "Client")).FullName)
            {
                SourcePath = Path.Combine(SelectedFolder.FullPath, Constants.DefaultApplicationName)
            };
        }

        private ApplicationManifest CreateApplicationManifest()
        {
            // 4.0 in ClickOnceUtil4UI name. 
            const string DefaultFramework = "v4.0";

            return new ApplicationManifest(DefaultFramework)
            {
                SourcePath = Path.Combine(SelectedFolder.FullPath, Constants.DefaultManifestName)
            };
        }

        private void SelectedActionChanges(UserActions action)
        {
            if (action == UserActions.New || action == UserActions.Update)
            {
                var deploy = SelectedFolder.DeployManifest ?? CreateDeployManifest();
                var application = SelectedFolder.ApplicationManifest ?? CreateApplicationManifest();

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