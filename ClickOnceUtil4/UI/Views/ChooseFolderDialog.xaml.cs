namespace ClickOnceUtil4UI.UI.Views
{
    /// <summary>
    /// Interaction logic for ChooseFolderDialog.xaml
    /// </summary>
    public partial class ChooseFolderDialog
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="ChooseFolderDialog"/>.
        /// </summary>
        public ChooseFolderDialog(object dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}