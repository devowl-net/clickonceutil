using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ClickOnceUtil4UI.UI.Behaviors
{
    /// <summary>
    /// <see cref="Behavior{T}"/> for <see cref="PasswordBox"/>.
    /// </summary>
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for bindable property Password.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(PasswordBox.Password),
                typeof(string),
                typeof(PasswordBoxBehavior),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// Password value.
        /// </summary>
        public string Password
        {
            get
            {
                return (string)GetValue(PasswordProperty);
            }
            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += OnPasswordChanged;
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = AssociatedObject.Password;
        }
    }
}