using BookManagement.Services.AccountService;
using BookManagement.Services.AuthorService;
using BookManagement.Services.ReaderService;
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

namespace BookManagement.Views.Reader
{
    /// <summary>
    /// Interaction logic for CreateReaderWindow.xaml
    /// </summary>
    public partial class CreateReaderWindow : Window
    {
        private readonly AccountService _account;
        private readonly ReaderService _reader;
        public CreateReaderWindow()
        {
            InitializeComponent();
            _account = new AccountService();
            _reader = new ReaderService();
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string password = Password.Password;
            string fullname = Fullname.Text;
            string email = Email.Text;
            string phone = Phone.Text;
            string address = Address.Text;
            ///tu check validate
            bool isCreated = await _reader.CreateReaderAsync(new Entities.Account()
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
