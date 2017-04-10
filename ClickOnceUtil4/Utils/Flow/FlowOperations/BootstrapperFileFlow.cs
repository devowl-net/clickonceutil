using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClickOnceUtil4UI.Clickonce;
using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.Bootstrapper;

namespace ClickOnceUtil4UI.Utils.Flow.FlowOperations
{
    /// <summary>
    /// Create bootstrapper scenario.
    /// </summary>
    public class BootstrapperFileFlow : FlowBase
    {
        /// <summary>
        /// Constructor for <see cref="BootstrapperFileFlow"/>.
        /// </summary>
        public BootstrapperFileFlow() : base(UserActions.BootstrapperFile)
        {
        }

        /// <inheritdoc/>
        public override bool IsFlowApplicable(FolderTypes folderType, string fullPath)
        {
            return folderType == FolderTypes.ClickOnceApplication;
        }

        /// <inheritdoc/>
        public override bool Execute(Container container, out string errorString)
        {
            errorString = null;

            // C:\Program Files (x86)\Microsoft Visual Studio 14.0\SDK\Bootstrapper
            return true;
            /*
                <BootstrapperPackage Include=".NETFramework,Version=v4.0">
                  <Visible>False</Visible>
                  <ProductName>Microsoft .NET Framework 4 %28x86 and x64%29</ProductName>
                  <Install>true</Install>
                </BootstrapperPackage>
                <BootstrapperPackage Include=".NETFramework,Version=v4.0,KB2468871">
                  <Visible>False</Visible>
                  <ProductName>Microsoft .NET Framework 4 %28x86 and x64%29 and Update for .NET Framework 4 %28KB2468871%29</ProductName>
                  <Install>true</Install>
                </BootstrapperPackage>
                <BootstrapperPackage Include="Microsoft.Net.Framework.3.5.SP1">
                  <Visible>False</Visible>
                  <ProductName>.NET Framework 3.5 SP1</ProductName>
                  <Install>false</Install>
                </BootstrapperPackage>
                <BootstrapperPackage Include="Microsoft.Windows.Installer.4.5">
                  <Visible>False</Visible>
                  <ProductName>Windows Installer 4.5</ProductName>
                  <Install>true</Install>
                </BootstrapperPackage>
            */
            //var builder = new BootstrapperBuilder
            //{
            //    Path = container.FullPath
            //};

            //var settings = new BuildSettings();
            //var product = new Product()
            //{
            //    ProductBuilder = 
            //};
            //ProductBuilder.
            //var q = new ProductBuilder(new Product());
            //settings.ProductBuilders.Add();
            //var result = builder.Build(settings);

            //return result.Succeeded;
        }

        /// <inheritdoc/>
        public override IEnumerable<InfoData> GetBuildInformation(Container container)
        {
            yield break;
        }
    }
}
