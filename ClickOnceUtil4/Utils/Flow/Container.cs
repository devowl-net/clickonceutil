using System.Security.Cryptography.X509Certificates;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Container for flow operations.
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Full path to root folder.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// <see cref="ApplicationManifest"/> instance.
        /// </summary>
        public ApplicationManifest Application { get; set; }

        /// <summary>
        /// <see cref="DeployManifest"/> instance.
        /// </summary>
        public DeployManifest Deploy { get; set; }

        /// <summary>
        /// Certificate object.
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// ClickOnce application version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// EntryPoint executable file.
        /// </summary>
        public string EntrypointPath { get; set; }

        /// <summary>
        /// Application name. 
        /// </summary>
        /// <remarks>
        /// Contains application name of executable file. Its required if more then one instance of application with 
        /// a same version needs to install on client. In this case each one application should have unique EntryPoint
        /// file name.
        /// </remarks>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Timestamp server URL.
        /// </summary>
        public string TimestampUrl { get; set; }
    }
}