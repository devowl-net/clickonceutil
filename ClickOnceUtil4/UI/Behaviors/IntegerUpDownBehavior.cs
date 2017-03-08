using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ClickOnceUtil4UI.UI.Behaviors
{
    /// <summary>
    /// <see cref="Behavior{T}"/> for integer input.
    /// </summary>
    public class IntegerUpDownBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Dependency property for integer value.
        /// </summary>
        public static readonly DependencyProperty IntegerValueProperty = DependencyProperty.Register(
            "IntegerValue",
            typeof(int),
            typeof(IntegerUpDownBehavior),
            new PropertyMetadata(default(int), IntegerValuePropertyChangedCallback));

        /// <summary>
        /// Dependency property for ButtonUp.
        /// </summary>
        public static readonly DependencyProperty ButtonUpProperty = DependencyProperty.Register(
            "ButtonUp",
            typeof(Button),
            typeof(IntegerUpDownBehavior),
            new PropertyMetadata(default(Button)));

        /// <summary>
        /// Dependency property for ButtonDown.
        /// </summary>
        public static readonly DependencyProperty ButtonDownProperty = DependencyProperty.Register(
            "ButtonDown",
            typeof(Button),
            typeof(IntegerUpDownBehavior),
            new PropertyMetadata(default(Button)));

        /// <summary>
        /// Integer value.
        /// </summary>
        public int IntegerValue
        {
            get
            {
                return (int)GetValue(IntegerValueProperty);
            }

            set
            {
                SetValue(IntegerValueProperty, value);
            }
        }

        /// <summary>
        /// Button up reference.
        /// </summary>
        public Button ButtonUp
        {
            get
            {
                return (Button)GetValue(ButtonUpProperty);
            }
            set
            {
                SetValue(ButtonUpProperty, value);
            }
        }

        /// <summary>
        /// Button down reference.
        /// </summary>
        public Button ButtonDown
        {
            get
            {
                return (Button)GetValue(ButtonDownProperty);
            }
            set
            {
                SetValue(ButtonDownProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObjectOnPreviewKeyDown;
            AssociatedObject.TextChanged += AssociatedObjectOnTextChanged;
            AssociatedObject.Loaded += (sender, args) =>
                                       {
                                           ButtonUp.Click += ButtonUpClick;
                                           ButtonDown.Click += ButtonDownClick;
                                       };
        }

        private static void IntegerValuePropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
        }

        private void AssociatedObjectOnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var dKeys = keyEventArgs.Key >= Key.D0 && keyEventArgs.Key <= Key.D9;
            var numKeys = keyEventArgs.Key >= Key.NumPad0 && keyEventArgs.Key <= Key.NumPad9;

            if (!dKeys && !numKeys)
            {
                keyEventArgs.Handled = true;
            }
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            IntegerValue--;
            AssociatedObject.Text = IntegerValue.ToString();
        }

        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            IntegerValue++;
            AssociatedObject.Text = IntegerValue.ToString();
        }

        private void AssociatedObjectOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            var text = AssociatedObject.Text;
            if (string.IsNullOrEmpty(text))
            {
                IntegerValue = default(int);
                return;
            }

            int buffer;
            if (!int.TryParse(text, out buffer))
            {
                IntegerValue = default(int);
            }
            else
            {
                IntegerValue = buffer;
            }
        }
    }
}