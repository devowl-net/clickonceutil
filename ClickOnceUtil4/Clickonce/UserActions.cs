using System.ComponentModel;

namespace ClickOnceUtil4UI.Clickonce
{
    /// <summary>
    /// Actions available for user.
    /// </summary>
    public enum UserActions
    {
        /// <summary>
        /// No actions, default value.
        /// </summary>
        [Description("No actions")]
        None,

        /// <summary>
        /// Create new application.
        /// </summary>
        [Description("Create new application")]
        New,

        /// <summary>
        /// Update application presets.
        /// </summary>
        [Description("Manual change application presets")]
        Update,

        /// <summary>
        /// Remove ClickOnce label.
        /// </summary>
        [Description("Remove ClickOnce label such as .deploy extensions and required files such as .application and .manifest")]
        Remove,

        /// <summary>
        /// Update application presets.
        /// </summary>
        [Description("Resigning ClickOnce files")]
        Resigning,

        /// <summary>
        /// Create bootstrapper file.
        /// </summary>
        [Description("Create bootstrapper file.")]
        BootstrapperFile,
    }
}