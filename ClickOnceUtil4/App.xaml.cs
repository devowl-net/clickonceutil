using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

using EmbendedResources = ClickOnceUtil4UI.Properties.Resources;
namespace ClickOnceUtil4UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly IDictionary<string, byte[]> _resourceAssemblies = new Dictionary<string, byte[]>()
        {
            { "System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", EmbendedResources.System_Windows_Interactivity },
            { "Microsoft.Web.Administration, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", EmbendedResources.Microsoft_Web_Administration }
        };

        /// <summary>
        /// Constructor for  <see cref="App"/>.
        /// </summary>
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;

            byte[] assemblyByteCode;
            if (_resourceAssemblies.TryGetValue(assemblyName, out assemblyByteCode))
            {
                return Assembly.Load(_resourceAssemblies[assemblyName]);
            }

            return null;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.ToString());
            args.Handled = true;
        }
    }
}
