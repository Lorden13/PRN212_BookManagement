using BookManagement.Entities;
using BookManagement.Services.AccountService;
using BookManagement.Services.AdminService;
using BookManagement.Services.AuthorService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookManagement.Views.Author
{
    /// <summary>
    /// Interaction logic for CreateAuthorWindow.xaml
    /// </summary>
    public partial class CreateAuthorWindow : Window
    {
        private readonly AccountService _account;
        private readonly AuthorService _author;
        public CreateAuthorWindow()
        {
            InitializeComponent();
            _account = new AccountService();
            _author = new AuthorService();
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string password = Password.Password;
            string fullname = Fullname.Text;
            string email = Email.Text;
            string phone = Phone.Text;
            string address = Address.Text;
            ///tu check validate
            bool isCreated = await _author.CreateAuthorAsync(new Entities.Account()
            {
                Address = address,
                Email = email,
                FullName = fullname,
                Password = password,
                Phone = phone,

            });
            if (isCreated)
            {
                Message.Text = "Tạo Thành Công";
            }
            else
            {
                Message.Text = "Tạo Thất Bại. Có thể do email bị trùng";
            }
        }

    }
}
