using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Controls
{
    public partial class Header : UserControl
    {
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(
                nameof(HeaderContent),
                typeof(object),
                typeof(Header),
                new PropertyMetadata(null));

        public object HeaderContent 
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        public static readonly DependencyProperty PageTitleProperty =
            DependencyProperty.Register(
                nameof(PageTitle),
                typeof(string),
                typeof(Header),
                new PropertyMetadata("Dashboard", OnPageTitleChanged));

        public string PageTitle
        {
            get => (string)GetValue(PageTitleProperty);
            set => SetValue(PageTitleProperty, value);
        }

        private static void OnPageTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Header header && header.txtPageTitle != null)
            {
                header.txtPageTitle.Text = e.NewValue?.ToString() ?? "Dashboard";
            }
        }

        public Header()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var user = BookManagement.Services.Utils.UserSession.CurrentUser;
            if (user != null)
            {
                txtUserName.Text = user.FullName;
                txtUserRole.Text = user.Role;
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    txtAvatarLetter.Text = user.FullName[0].ToString().ToUpper();
                }
            }
        }
    }
}
