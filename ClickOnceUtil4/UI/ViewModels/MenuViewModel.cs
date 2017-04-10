using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Prism;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// View model for <see cref="MainWindow"/> menu.
    /// </summary>
    public class MenuViewModel
    {
        private readonly MainWindowViewModel _mainViewModel;

        private readonly IDictionary<string, string> _helpLinks = new Dictionary<string, string>()
        {
            { "ApplicationColumn", "https://msdn.microsoft.com/en-us/library/microsoft.build.tasks.deployment.manifestutilities.applicationmanifest_properties.aspx" },
            { "DeployColumn", "https://msdn.microsoft.com/en-us/library/microsoft.build.tasks.deployment.manifestutilities.deploymanifest_properties.aspx" },
            { "SecurityAndDeploy", "https://msdn.microsoft.com/en-us/library/ms228995.aspx" },
            { "CreateApplicationWithoutManifest", "http://stackoverflow.com/questions/5337458/error-deploying-clickonce-application-reference-in-the-manifest-does-not-match" },
            { "ClickOnceStrongName", "https://msdn.microsoft.com/en-us/library/aa730868(v=vs.80).aspx" },
            { "NetFrameworkHotfix", "https://connect.microsoft.com/VisualStudio/feedback/details/754487/mage-exe-hashes-with-sha1-but-maintains-to-hash-with-sha256" },
            { "VirtualDirectoryDeploymentUrl", "https://msdn.microsoft.com/en-us/library/bb763173.aspx" },
            { "DeployExtension", "http://stackoverflow.com/questions/25777441/use-mage-exe-to-create-a-clickonce-deployment-manifest-for-deploy-files" }
        };

        /// <summary>
        /// Constructor for <see cref="MenuViewModel"/>.
        /// </summary>
        public MenuViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            AboutCommand = new DelegateCommand(AboutHandler);
            CloseCommand = new DelegateCommand(CloseHandler);
            ShowHelpCommand = new DelegateCommand(ShowHelpHandler);
        }

        /// <summary>
        /// About command handler.
        /// </summary>
        public DelegateCommand AboutCommand { get; private set; }

        /// <summary>
        /// Show help command handler.
        /// </summary>
        public DelegateCommand ShowHelpCommand { get; private set; }
        
        /// <summary>
        /// Close command handler.
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        private void CloseHandler(object obj)
        {
            Application.Current.Shutdown();
        }

        private void AboutHandler(object obj)
        {
            new AboutWindow { Owner = Application.Current.MainWindow }.ShowDialog();
        }

        private void ShowHelpHandler(object obj)
        {
            if (obj == null)
            {
                MessageBox.Show("Help not founded");
                return;
            }

            string linkKey = obj.ToString();
            Process.Start(_helpLinks[linkKey]);
        }
    }
}