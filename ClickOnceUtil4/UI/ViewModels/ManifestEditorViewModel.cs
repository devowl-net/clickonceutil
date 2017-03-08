using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="ManifestEditorView"/>.
    /// </summary>
    public class ManifestEditorViewModel<TManifest>
        where TManifest : Manifest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manifest">Target manifest object.</param>
        public ManifestEditorViewModel(TManifest manifest)
        {
            if (manifest == null)
            {
                throw new ArgumentNullException(nameof(manifest));
            }

            Manifest = manifest;
            var publicProperties =
                typeof(TManifest).GetProperties()
                    .Where(property => property.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Any());

            Properties = new ObservableCollection<PropertyObject>(publicProperties.Select(CreatePropertyObject));
        }

        /// <summary>
        /// Manifest object.
        /// </summary>
        public TManifest Manifest { get; private set; }

        /// <summary>
        /// List of <see cref="PropertyObject"/> for editing values.
        /// </summary>
        public ObservableCollection<PropertyObject> Properties { get; private set; }

        private PropertyObject CreatePropertyObject(PropertyInfo property)
        {
            var description = typeof(TManifest) == property.DeclaringType
                ? BuildTasks40SummaryUtils.ReadPropertySummary<TManifest>(property.Name)
                : BuildTasks40SummaryUtils.ReadPropertySummary<Manifest>(property.Name);

            return new PropertyObject(Manifest, property, description);
        }
    }
}