using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using ClickOnceUtil4UI.UI.Models;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.UI.Behaviors
{
    /// <summary>
    /// <see cref="StyleSelector"/> for Manifest properties.
    /// </summary>
    public class PropertyTypeStyleSelector : StyleSelector
    {
        private readonly Dictionary<Type, string> _mapping = new Dictionary<Type, string>()
        {
            { typeof(string), "StringProperty" },
            { typeof(bool), "BooleanProperty" },
            { typeof(int), "IntegerProperty" }
        };

        /// <inheritdoc/>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var propertyObject = item as PropertyObject;
            if (propertyObject == null)
            {
                // Designer show exception
                return null;
            }
            
            return ReadStyle(container, propertyObject);
        }
        
        private Style ReadStyle(DependencyObject container, PropertyObject propertyObject)
        {
            Type propertyType = propertyObject.PropertyType;
            var userControl = FindParent<UserControl>(container);
            var resources = userControl.Resources;

            // Enum field
            string key = propertyType.IsEnum ? "EnumProperty" : null;
            
            // String URL field
            if (propertyType == typeof(string) && propertyObject.PropertyName.Contains("Url"))
            {
                key = "UriStringProperty";
            }

            // Last check
            if (key == null && !_mapping.TryGetValue(propertyType, out key))
            {
                return (Style)resources["Empty"];
            }

            return (Style)resources[key];
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}