using System;
using System.ComponentModel;
using System.Reflection;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// <see cref="Enum" /> extension for <see cref="DescriptionAttribute"/>.
    /// </summary>
    public static class DisplayExtension
    {
        /// <summary>
        /// Returns enum value description.
        /// </summary>
        /// <param name="enum">Base <see cref="Enum"/> class.</param>
        /// <returns>Description text.</returns>
        public static string GetDescription(this Enum @enum)
        {
            FieldInfo field = @enum.GetType().GetField(@enum.ToString());

            var descriptionAttribute = field.FindAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }

            return string.Empty;
        }

        private static T FindAttribute<T>(this MemberInfo field) where T : Attribute
        {
            return Attribute.GetCustomAttribute(field, typeof(T)) as T;
        }
    }
}