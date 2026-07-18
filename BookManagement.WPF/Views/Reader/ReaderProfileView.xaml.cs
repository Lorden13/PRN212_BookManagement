using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using BookManagement.WPF.Services.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using BookManagement.Services.Utils;



namespace BookManagement.Views.Reader
{
    public partial class ReaderProfileView : UserControl
    {
        private readonly IReaderService _readerService;

        public ReaderProfileView()
        {
            InitializeComponent();

            _readerService = App.Current.Services.GetRequiredService<IReaderService>();

            Loaded += ReaderProfileView_Loaded;
        }

        private void ReaderProfileView_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user != null)
            {
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
                MessageBox.Show("Vui lòng nhập họ tên.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var readerModel = new ReaderModel
                {
                    Id = user.AccountId,
                    Name = name,
                    Email = email,
                    Phone = phone,
                    Status = user.IsActive ? "Active" : "Inactive"
                };

                _readerService.UpdateProfile(readerModel);
                MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật thông tin: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
