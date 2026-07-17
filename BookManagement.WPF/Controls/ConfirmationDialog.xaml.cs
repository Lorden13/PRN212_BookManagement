using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class ConfirmationDialog : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(ConfirmationDialog), new PropertyMetadata(false));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ConfirmationDialog), new PropertyMetadata("Xác nhận"));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(ConfirmationDialog), new PropertyMetadata(string.Empty));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public event EventHandler Confirmed;
        public event EventHandler Cancelled;

        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
            Confirmed?.Invoke(this, EventArgs.Empty);
        }
    }
}
