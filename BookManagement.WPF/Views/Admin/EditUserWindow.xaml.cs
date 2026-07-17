using System.Windows;
using BookManagement.ViewModels.Admin;

namespace BookManagement.Views.Admin
{
    public partial class EditUserWindow : Window
    {
        public EditUserWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EditUserViewModel oldVm)
            {
                oldVm.CloseRequested -= OnCloseRequested;
            }

            if (e.NewValue is EditUserViewModel newVm)
            {
                newVm.CloseRequested += OnCloseRequested;
            }
        }

        private void OnCloseRequested(bool result)
        {
            DialogResult = result;
        }
    }
}
