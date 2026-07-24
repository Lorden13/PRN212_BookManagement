using BookManagement.Views.Common;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BookManagement.Views.Admin
{
    public partial class AdminDashboard : Page
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var user = BookManagement.Services.Utils.UserSession.CurrentUser;
            if (user != null)
            {
                txtSidebarUserName.Text = user.FullName;
                txtSidebarUserEmail.Text = user.Email;
                if (!string.IsNullOrEmpty(user.FullName))
                {
                    txtSidebarUserLetter.Text = user.FullName[0].ToString().ToUpper();
                }
            }

            // Register Content Frame for Navigation Service
            BookManagement.Services.Navigation.NavigationService.Instance.RegisterContentFrame(frmContent);

            // Default navigate to Home page
            lstMenu.SelectedIndex = 0;
            frmContent.Navigate(new AdminHomeView());
            header.PageTitle = "Trang chủ";
        }

        private void lstMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (frmContent == null || header == null) return;

            if (lstMenu.SelectedItem is ListBoxItem selectedItem)
            {
                string tag = selectedItem.Tag?.ToString() ?? string.Empty;
                switch (tag)
                {
                    case "Home":
                        frmContent.Navigate(new AdminHomeView());
                        header.PageTitle = "Trang chủ";
                        break;
                    case "Users":
                        frmContent.Navigate(new AdminUsersView());
                        header.PageTitle = "Quản lý người dùng";
                        break;
                    case "PendingBooks":
                        frmContent.Navigate(new AdminPendingBooksView());
                        header.PageTitle = "Sách chưa duyệt";
                        break;
                    case "AllBooks":
                        frmContent.Navigate(new AdminAllBooksView());
                        header.PageTitle = "Tất cả đầu sách";
                        break;

                    case "ChangePassword":
                        frmContent.Navigate(new ChangePasswordView());
                        header.PageTitle = "Đổi mật khẩu";
                        break;
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            BookManagement.Services.Utils.UserSession.CurrentUser = null!;
            Services.Navigation.NavigationService.Instance.NavigateMain(new Login());
        }
    }
}
