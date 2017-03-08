using System;
using System.Globalization;
using System.Xml;

using ClickOnceUtil4UI.Properties;

namespace ClickOnceUtil4UI.Utils
{
    public static class BuildTasks40SummaryUtils
    {
        private static XmlDocument _buildTasks40XmlDocument = null;

        /// <summary>
        /// Reads inner text for property summary.
        /// </summary>
        /// <typeparam name="TSource">Property owner type.</typeparam>
        /// <param name="propertyName">Property name.</param>
        /// <returns>Formatted summary text, otherwise null.</returns>
        public static string ReadPropertySummary<TSource>(string propertyName)
        {
            if (_buildTasks40XmlDocument == null)
            {
                _buildTasks40XmlDocument = new XmlDocument();
                _buildTasks40XmlDocument.LoadXml(Resources.Microsoft_Build_Tasks_v4_0);
            }

            var xmlDocument = _buildTasks40XmlDocument;
            var type = typeof(TSource);
            var fullPropertyPath = $"P:{type.Namespace}.{type.Name}.{propertyName}";
            var xmlNode = xmlDocument.SelectSingleNode($"//member[@name='{fullPropertyPath}']");

            if (xmlNode == null)
            {
                return null;
            }

            return Preformat(xmlNode.InnerText);

            /*
             <doc>
              <assembly>
                <name>Microsoft.Build.Tasks.v4.0</name>
              </assembly>
              <members>
                <member name="T:Microsoft.Build.Tasks.AL">
                  <summary>Implements the AL task. Use the AL element in your project file to create and execute this task. For usage and parameter information, see AL (Assembly Linker) Task.</summary>
                </member>
                <member name="M:Microsoft.Build.Tasks.AL.#ctor">
                  <summary>Initializes a new instance of the <see cref="T:Microsoft.Build.Tasks.AL" /> class.</summary>
                </member>
                <member name="M:Microsoft.Build.Tasks.AL.AddResponseFileCommands(Microsoft.Build.Tasks.CommandLineBuilderExtension)">
                  <summary>Fills the specified <paramref name="commandLine" /> parameter with switches and other information that can go into a response file.</summary>
                  <param name="commandLine">Command line builder to add arguments to.</param>
                </member>
             */
        }

        private static string Preformat(string sourceString)
        {
            var prefixs = new[]
            {
                "Gets or sets ",
                "Gets "
            };

            foreach (var prefix in prefixs)
            {
                if (sourceString.StartsWith(prefix))
                {
                    sourceString = sourceString.Substring(prefix.Length);
                    break;
                }
            }

            var firstChar = sourceString[0];
            if (!char.IsUpper(firstChar))
            {
                return char.ToUpper(firstChar) + sourceString.Substring(1);
            }

            return sourceString;
        }
    }
}