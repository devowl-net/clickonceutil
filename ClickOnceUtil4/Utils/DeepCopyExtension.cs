using System;
using System.Reflection;

namespace ClickOnceUtil4UI.Utils
{
    /// <summary>
    /// Deep copy extension.
    /// </summary>
    public static class DeepCopyExtension
    {
        /// <summary>
        /// Create deep copy of object.
        /// </summary>
        /// <param name="obj">Source object reference.</param>
        /// <returns>Returns new object.</returns> 
        public static object Copy(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            Type type = obj.GetType();

            if (type.IsValueType || type == typeof(string))
            {
                return obj;
            }

            if (type.IsArray)
            {
                Type elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(Copy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }

            if (type.IsClass)
            {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                    {
                        continue;
                    }
                    field.SetValue(toret, Copy(fieldValue));
                }
                return toret;
            }

            throw new ArgumentException("Unknown type");
        }
    }
}