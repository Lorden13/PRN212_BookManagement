using BookManagement.Views.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace BookManagement.Views.Author
{
    public partial class AuthorDashboard : Page
    {
        public AuthorDashboard()
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

            // Default navigate to Home page
            lstMenu.SelectedIndex = 0;
            frmContent.Navigate(new AuthorHomeView());
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
                        frmContent.Navigate(new AuthorHomeView());
                        header.PageTitle = "Trang chủ";
                        break;
                    case "Books":
                        frmContent.Navigate(new AuthorBooksView());
                        header.PageTitle = "Quản lý sách";
                        break;
                    case "CreateBook":
                        frmContent.Navigate(new AuthorCreateBookView());
                        header.PageTitle = "Thêm sách mới";
                        break;
                    case "ReviewHistory":
                        frmContent.Navigate(new AuthorReviewHistoryView());
                        header.PageTitle = "Lịch sử đánh giá";
                        break;
                    case "Profile":
                        frmContent.Navigate(new AuthorProfileView());
                        header.PageTitle = "Thông tin cá nhân";
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
