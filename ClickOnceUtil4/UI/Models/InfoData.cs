namespace ClickOnceUtil4UI.UI.Models
{
    /// <summary>
    /// Information data object.
    /// </summary>
    public class InfoData
    {
        /// <summary>
        /// Constructor for <see cref="InfoData"/>.
        /// </summary>
        public InfoData(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Short name of information.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description text.
        /// </summary>
        public string Description { get; private set; }
    }
}