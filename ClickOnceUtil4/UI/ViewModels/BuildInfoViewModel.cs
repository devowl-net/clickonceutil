using System.Collections.Generic;
using System.Windows;

using ClickOnceUtil4UI.UI.Models;
using ClickOnceUtil4UI.UI.Views;
using ClickOnceUtil4UI.Utils.Prism;

namespace ClickOnceUtil4UI.UI.ViewModels
{
    /// <summary>
    /// View model for <see cref="BuildInfoView"/>.
    /// </summary>
    public class BuildInfoViewModel : NotificationObject
    {
        /// <summary>
        /// Constructor for <see cref="BuildInfoViewModel"/>.
        /// </summary>
        public BuildInfoViewModel(IEnumerable<InfoData> info)
        {
            Info = info;
            BuildCommand = new DelegateCommand(BuildHandler);
            CancelCommand = new DelegateCommand(CancelHandler);
        }

        /// <summary>
        /// Information key pairs. Key - name, Value description.
        /// </summary>
        public IEnumerable<InfoData> Info { get; }

        /// <summary>
        /// Okay command.
        /// </summary>
        public DelegateCommand BuildCommand { get; }

        /// <summary>
        /// Cancel command.
        /// </summary>
        public DelegateCommand CancelCommand { get; }

        private void CancelHandler(object obj)
        {
            DialogResult(obj, false);
        }

        private void BuildHandler(object obj)
        {
            DialogResult(obj, true);
        }

        private void DialogResult(object obj, bool? result)
        {
            Window window = (Window)obj;
            if (window != null)
            {
                window.DialogResult = result;
            }
        }
    }
}