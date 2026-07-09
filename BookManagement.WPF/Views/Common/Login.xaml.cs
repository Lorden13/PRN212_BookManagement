using BookManagement.Services.AccessTokenService;
using BookManagement.Services.RoleService;
using BookManagement.SQLite;
using BookManagement.WPF.Entities;
using BookManagement.WPF.Services.AccountService;
using BookManagement.WPF.Services.AdminService;
using BookManagement.WPF.Services.AuthorSetvice;
using BookManagement.WPF.Services.ReaderService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagement.Views.Common
{
    public partial class Login : Page
    {
        private readonly AuthorService _author;
        private readonly ReaderService _reader;
        private readonly AccountService _account;
        private readonly AdminService _admin;
        private readonly RoleService _role;
        private readonly AccessTokenService _token;
        public Login()
        {
            InitializeComponent();

            _author = new AuthorService();
            _reader = new ReaderService();
            _account = new AccountService();
            _admin = new AdminService();
            _role = new RoleService();
            _token = new AccessTokenService();
            registerRole.ItemsSource = new List<string>()
            {
                "Reader",
                "Author",
                "Admin"
            };
            loginRole.ItemsSource = new List<string>()
            {
                "Reader",
                "Author",
                "Admin"
            };
        }

        

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void txtPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (txtPassword != null && txtPassword.Password != textBox.Text)
                {
                    txtPassword.Password = textBox.Text;
                }
            }
        }

        private bool _isUpdatingPassword = false;
        private void txtRegisterPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingPassword) return;

            if (sender is TextBox textBox && txtRegisterPassword != null)
            {
                _isUpdatingPassword = true;
                txtRegisterPassword.Password = textBox.Text;
                _isUpdatingPassword = false;
            }
        }

        private void txtRegisterConfirmPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingPassword) return;

            if (sender is TextBox textBox && txtRegisterConfirmPassword != null)
            {
                _isUpdatingPassword = true;
                txtRegisterConfirmPassword.Password = textBox.Text;
                _isUpdatingPassword = false;
            }
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new System.Text.RegularExpressions.Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            string fullFullname = registerFullname.Text;
            string email = registerEmail.Text;
            string address = registerAddress.Text;
            string password = txtRegisterPassword.Password;
            string confirmPassword = txtRegisterConfirmPasswordPlain.Text;
            string phone = registerPhone.Text;
            string role = (string)registerRole.SelectedItem;
            if (role == "Author")
            {
                if (password == confirmPassword)
                {
                    bool isCreated = await _author.CreateAuthorAsync(new WPF.Entities.Account
                    {
                        Email = email,
                        Password = password,
                        FullName = fullFullname,
                        Address = address,
                        Phone = phone,
                        IsActive = true

                    });
                    if (isCreated)
                    {
                        Message.Text = "Tạo thành công";
                      
                    }
                    else
                    {
                        Message.Text = "Tạo thất bại. Vui lòng kiểm tra lại thông tin";
                    }
                }
                else
                {
                    Message.Text = "Tạo thất bại. Vui lòng kiểm tra lại mật khẩu";
                }
            }
            else if (role == "Reader")
            {
                if (password == confirmPassword)
                {
                    bool isCreated = await _reader.CreateReaderAsync(new WPF.Entities.Account
                    {
                        Email = email,
                        Password = password,
                        FullName = fullFullname,
                        Address = address,
                        Phone = phone,
                        IsActive = true

                    });
                    if (isCreated)
                    {
                        Message.Text = "Tạo thành công";
                       
                    }
                    else
                    {
                        Message.Text = "Tạo thất bại. Vui lòng kiểm tra lại thông tin";
                    }
                }
                else
                {
                    Message.Text = "Tạo thất bại. Vui lòng kiểm tra lại mật khẩu";
                }
            }
           
            else
            {
                Message.Text = "Tạo Thất Bại. Có thể do email bị trùng";
            }

        }

        private async void ButtonLogin_Clicked(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string roleChoice = (string)loginRole.SelectedItem;
            Role role = null;
            if (roleChoice == "Author")
            {
                role = await _role.GetByNameAsync("Author");
            }
            if (roleChoice == "Reader")
            {
                role = await _role.GetByNameAsync("Reader");

            }
            if (roleChoice == "Admin")
            {
                role = await _role.GetByNameAsync("Admin");

            }
            Account accountDb = await _account.CheckLoginAsync(email, password, role.RoleId);
            if (accountDb != null)
            {
                string accessToken = await _token.GenerateAccessTokenAsync(accountDb.AccountId);
                UserSecretContext sqliteContext = new UserSecretContext();
                SavedToken token = await sqliteContext.SavedTokens.FirstOrDefaultAsync();
                if (token == null)
                {
                    SavedToken newDb = new SavedToken()
                    {
                        Id = Guid.NewGuid().ToString(),
                        TokenValue = accessToken
                    };
                    await sqliteContext.SavedTokens.AddAsync(newDb);
                }
                else
                {
                    token.TokenValue = accessToken;
                    sqliteContext.SavedTokens.Update(token);
                }
                await sqliteContext.SaveChangesAsync();
               
                if(role.RoleName == "Admin")
                {
                    Services.Navigation.NavigationService.Instance.NavigateMain(new AdminDashboard());
                }
                if(role.RoleName == "Author")
                {
                    Services.Navigation.NavigationService.Instance.NavigateMain(new AuthorDashboard());
                }

                if (role.RoleName == "Reader")
                {
                    Services.Navigation.NavigationService.Instance.NavigateMain(new ReaderDashboard());
                }

            }
            else
            {
                btnLogin.IsEnabled = true;
                txtError.Visibility = Visibility.Visible;
                txtError.Text = "Username or password is not correct";
            }
        }

  

        private void registerLink_Click(object sender, RoutedEventArgs e)
        {
            registerForm.Visibility = Visibility.Visible;
            loginForm.Visibility = Visibility.Hidden;
        }

        private void loginLink_Click(object sender, RoutedEventArgs e)
        {
            registerForm.Visibility = Visibility.Hidden;
            loginForm.Visibility = Visibility.Visible;
        }
    }
}
