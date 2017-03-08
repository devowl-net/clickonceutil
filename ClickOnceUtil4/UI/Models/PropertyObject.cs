using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOnceUtil4UI.UI.Models
{
    /// <summary>
    /// Property editor object.
    /// </summary>
    public class PropertyObject : IDataErrorInfo
    {
        private readonly object _sourceObject;

        private readonly PropertyInfo _property;

        private bool _successInput = true;

        private string _currentErrorText;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceObject">Source object.</param>
        /// <param name="property">Property info.</param>
        /// <param name="description">Property description.</param>
        public PropertyObject(object sourceObject, PropertyInfo property, string description)
        {
            _sourceObject = sourceObject;
            _property = property;
            if (sourceObject == null)
            {
                throw new ArgumentNullException(nameof(sourceObject));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            Description = description;
            PropertyType = property.PropertyType;
        }

        /// <summary>
        /// String value binding.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// <see cref="Type"/> of <see cref="PropertyName"/>.
        /// </summary>
        public Type PropertyType { get; }

        /// <summary>
        /// Current property name.
        /// </summary>
        public string PropertyName => _property.Name;

        /// <summary>
        /// String value binding.
        /// </summary>
        public string StringValue
        {
            get
            {
                return (string)_property.GetValue(_sourceObject, null);
            }

            set
            {
                SetPropertyValue(value);
            }
        }

        /// <summary>
        /// Boolean value binding.
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                return (bool)_property.GetValue(_sourceObject, null);
            }

            set
            {
                _property.SetValue(_sourceObject, value, null);
            }
        }

        /// <summary>
        /// Enum value binding.
        /// </summary>
        public IEnumerable<object> EnumValues
        {
            get
            {
                return Enum.GetValues(_property.PropertyType).Cast<object>();
            }
        }

        /// <summary>
        /// Selected enum value.
        /// </summary>
        public Enum SelectedEnumValue
        {
            get
            {
                return (Enum)_property.GetValue(_sourceObject, null);
            }

            set
            {
                _property.SetValue(_sourceObject, value, null);
            }
        }

        /// <summary>
        /// Integer value.
        /// </summary>
        public int IntegerValue
        {
            get
            {
                return (int)_property.GetValue(_sourceObject, null);
            }

            set
            {
                _property.SetValue(_sourceObject, value, null);
            }
        }

        /// <inheritdoc/>
        public string Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Assembly reference list.
        /// </summary>
        public ObservableCollection<AssemblyReference> AssemblyReferences { get; } =
            new ObservableCollection<AssemblyReference>();

        /// <summary>
        /// Selected assembly reference.
        /// </summary>
        public AssemblyReference SelectedAssemblyReference
        {
            get
            {
                return (AssemblyReference)_property.GetValue(_sourceObject, null);
            }

            set
            {
                _property.SetValue(_sourceObject, value, null);
            }
        }

        /// <inheritdoc/>
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(StringValue) && PropertyName.Contains("Url"))
                {
                    var stringValue = (StringValue ?? string.Empty).Trim();
                    if (stringValue.StartsWith("\\\\") || string.IsNullOrEmpty(stringValue))
                    {
                        return null;
                    }

                    Uri result;
                    var supportingSchemes = new[]
                    {
                        Uri.UriSchemeHttp,
                        Uri.UriSchemeHttps
                    };

                    if (!Uri.TryCreate(stringValue, UriKind.RelativeOrAbsolute, out result) || !result.IsAbsoluteUri ||
                        !supportingSchemes.Contains(result.Scheme))
                    {
                        return "Warring: Incorrect HTTP(s) format.";
                    }
                }

                if (!_successInput)
                {
                    return _currentErrorText;
                }

                return null;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            // Debug print
            return $"{PropertyName} + {PropertyType}";
        }

        private void SetPropertyValue(string value)
        {
            try
            {
                _property.SetValue(_sourceObject, value, null);
                _currentErrorText = null;
                _successInput = true;
            }
            catch (TargetInvocationException exception)
            {
                _successInput = false;
                _currentErrorText = exception.InnerException?.Message;
            }
        }
    }
}