using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnAttachChanged));

        public static readonly DependencyProperty HasPasswordProperty =
            DependencyProperty.RegisterAttached("HasPassword", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false));

        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }

        public static bool GetHasPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasPasswordProperty);
        }

        public static void SetHasPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(HasPasswordProperty, value);
        }

        private static void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if ((bool)e.OldValue)
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }

                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                    // Initial check
                    SetHasPassword(passwordBox, passwordBox.Password.Length > 0);
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetHasPassword(passwordBox, passwordBox.Password.Length > 0);
            }
        }
    }
}
