using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Services.Utils;


namespace BookManagement.Views.Author
{
    public partial class AuthorProfileView : UserControl
    {
        private readonly IAuthorService _authorService;

        public AuthorProfileView()
        {
            InitializeComponent();

            _authorService = App.Current.Services.GetRequiredService<IAuthorService>();

            Loaded += AuthorProfileView_Loaded;
        }

        private void AuthorProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user != null)
            {
                txtProfileTitle.Text = user.FullName;
                txtName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.Phone ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Vui lòng nhập họ tên tác giả.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var authorModel = new AuthorModel
                {
                    Id = user.AccountId,
                    Name = name,
                    Email = email,
                    Phone = phone
                };

                _authorService.UpdateProfile(authorModel);
                txtProfileTitle.Text = name;

                // Sync UserSession info
                user.FullName = name;
                user.Email = email;
                user.Phone = phone;

                MessageBox.Show("Cập nhật thông tin tác giả thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
