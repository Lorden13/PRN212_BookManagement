using BookManagement.Views.Common;
using BookManagement.WPF.Views.Reader;
using System;
using System.Windows;
using System.Windows.Controls;
namespace BookManagement.Views.Reader
{
    public partial class ReaderDashboard : Page
    {
        public ReaderDashboard()
        {
            InitializeComponent();  //lỗi  ambigous 
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
            frmContent.Navigate(new ReaderHomeView());
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
                        frmContent.Navigate(new ReaderHomeView());
                        header.PageTitle = "Trang chủ";
                        break;
                    case "Books":
                        frmContent.Navigate(new ReaderBooksView());
                        header.PageTitle = "Khám phá sách";
                        break;
                    case "Favorites":
                        frmContent.Navigate(new ReaderFavoriteView());
                        header.PageTitle = "Sách yêu thích";
                        break;
                    case "Cart":
                        frmContent.Navigate(new ReaderCartView());
                        header.PageTitle = "Giỏ hàng";
                        break;
                    case "MyBooks":
                        frmContent.Navigate(new PurchaseHistoryView());
                        header.PageTitle = "Sách của tôi";
                        break;

                    case "Profile":
                        frmContent.Navigate(new UserProfileView());
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

        private void header_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
