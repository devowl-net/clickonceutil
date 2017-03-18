using System;
using System.Collections.Generic;
using System.IO;

using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.Utils.Flow
{
    /// <summary>
    /// Methods for getting info about manifests.
    /// </summary>
    public static class InfoUtils
    {
        /// <summary>
        /// Reads important information from <see cref="DeployManifest"/>.
        /// </summary>
        /// <param name="deploy"><see cref="DeployManifest"/> instance.</param>
        /// <returns><see cref="InfoData"/> items.</returns>
        public static IEnumerable<InfoData> GetDeployInfoData(DeployManifest deploy)
        {
            yield return
                new InfoData(
                    nameof(deploy.DeploymentUrl),
                    $"Your application hosting service must publishing your folder: \"{Environment.NewLine + Path.GetDirectoryName(deploy.SourcePath) + Environment.NewLine}\" and clients will try to activate your application from URI: {deploy.DeploymentUrl}")
                ;

            yield return
                new InfoData(
                    nameof(deploy.MapFileExtensions),
                    deploy.MapFileExtensions
                        ? "All application files will be renamed from [file.ext] to [file.ext.deploy]. Its useful if your publishing service have download files restrictions."
                        : "Be sure that your publishing service have not file restrictions for your files.");

            if (deploy.TrustUrlParameters)
            {
                yield return
                    new InfoData(
                        nameof(deploy.TrustUrlParameters),
                        "Any URL activation parameters will be passed to your application entry point as command line arguments.")
                    ;
            }

            if (!deploy.UpdateEnabled)
            {
                yield return new InfoData(nameof(deploy.UpdateEnabled), "Your application will not be updatable.", true);
            }

            yield return
                new InfoData(
                    nameof(deploy.UpdateMode),
                    deploy.UpdateMode == UpdateMode.Foreground
                        ? "Your application updating before launch."
                        : "Your application starts with a current copy and downloading new copy while works.");
        }

        /// <summary>
        /// Reads important information from <see cref="ApplicationManifest"/>.
        /// </summary>
        /// <param name="application"><see cref="ApplicationManifest"/> instance.</param>
        /// <returns><see cref="InfoData"/> items.</returns>
        public static IEnumerable<InfoData> GetApplicationInfoData(ApplicationManifest application)
        {
            if (!application.IsClickOnceManifest)
            {
                yield return new InfoData(nameof(application.IsClickOnceManifest), "Your are going to create ClickOnceApplication for none managed Win32 application.", true);
            }

            if (string.IsNullOrWhiteSpace(application.SuiteName))
            {
                yield return new InfoData(nameof(application.SuiteName), "No name of the folder on the Start menu where the application is located after ClickOnce deployment. Your name will be application name.", true);
            }
        }
    }
}