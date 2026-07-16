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
        private readonly BookManagement.WPF.Services.AuthorSetvice.AuthorService _author;
        private readonly BookManagement.WPF.Services.ReaderService.ReaderService _reader;
        private readonly AccountService _account;
        private readonly AdminService _admin;
        private readonly RoleService _role;
        private readonly AccessTokenService _token;
        public Login()
        {
            InitializeComponent();

            var vm = App.Current.Services.GetRequiredService<LoginViewModel>();
            DataContext = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
            _author = new BookManagement.WPF.Services.AuthorSetvice.AuthorService();
            _reader = new BookManagement.WPF.Services.ReaderService.ReaderService();

            _account = new AccountService();
            _admin = new AdminService();
            _role = new RoleService();
            _token = new AccessTokenService();
            registerRole.ItemsSource = new List<string>()
            {
                "Reader",
                "Author"
            };
            loginRole.ItemsSource = new List<string>()
            {
                "Reader",
                "Author",
                "Admin"
            };
        }

        

        private bool _isUpdatingPassword = false;

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isUpdatingPassword) return;
            if (sender is not PasswordBox passwordBox) return;

            _isUpdatingPassword = true;
            if (passwordBox == txtPassword && txtPasswordPlain != null)
            {
                txtPasswordPlain.Text = passwordBox.Password;
            }
            else if (passwordBox == txtRegisterPassword && txtRegisterPasswordPlain != null)
            {
                txtRegisterPasswordPlain.Text = passwordBox.Password;
            }
            else if (passwordBox == txtRegisterConfirmPassword && txtRegisterConfirmPasswordPlain != null)
            {
                txtRegisterConfirmPasswordPlain.Text = passwordBox.Password;
            }
            _isUpdatingPassword = false;
        }

        private void txtPasswordPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingPassword) return;

            if (sender is TextBox textBox && txtPassword != null)
            {
                _isUpdatingPassword = true;
                txtPassword.Password = textBox.Text;
                _isUpdatingPassword = false;
            }
        }
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

        private bool IsValidRegisterEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearRegisterErrors()
        {
            errFullName.Text = string.Empty;
            errEmail.Text = string.Empty;
            errPhone.Text = string.Empty;
            errAddress.Text = string.Empty;
            errPassword.Text = string.Empty;
            errConfirmPassword.Text = string.Empty;
            errRole.Text = string.Empty;
            Message.Text = string.Empty;
        }

        private async void Button_Click_Register(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearRegisterErrors();

                string fullFullname = registerFullname.Text;
                string email = registerEmail.Text;
                string address = registerAddress.Text;
                string password = txtRegisterPassword.Password;
                string confirmPassword = txtRegisterConfirmPasswordPlain.Text;
                string phone = registerPhone.Text;
                string role = (string)registerRole.SelectedItem;

                if (string.IsNullOrEmpty(role))
                {
                    errRole.Text = "Vui lòng chọn vai trò.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(fullFullname))
                {
                    errFullName.Text = "Vui lòng nhập họ và tên.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    errEmail.Text = "Vui lòng nhập email.";
                    return;
                }

                if (!IsValidRegisterEmail(email))
                {
                    errEmail.Text = "Email không hợp lệ. Vui lòng nhập đúng định dạng.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(phone))
                {
                    errPhone.Text = "Vui lòng nhập số điện thoại.";
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, "^[0-9]+$"))
                {
                    errPhone.Text = "Số điện thoại chỉ được chứa các chữ số.";
                    return;
                }

                if (phone.Length > 11)
                {
                    errPhone.Text = "Số điện thoại không được vượt quá 11 chữ số.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(address))
                {
                    errAddress.Text = "Vui lòng nhập địa chỉ.";
                    return;
                }

                if (address.Length > 255)
                {
                    errAddress.Text = "Địa chỉ không được vượt quá 255 ký tự.";
                    return;
                }

                if (password.Length < 6)
                {
                    errPassword.Text = "Mật khẩu phải có tối thiểu 6 ký tự.";
                    return;
                }

                if (password != confirmPassword)
                {
                    errConfirmPassword.Text = "Mật khẩu xác nhận không khớp.";
                    return;
                }

                var newAccount = new WPF.Entities.Account
                {
                    Email = email,
                    Password = password,
                    FullName = fullFullname,
                    Address = address,
                    Phone = phone,
                    IsActive = true
                };

                bool isCreated = role == "Author"
                    ? await _author.CreateAuthorAsync(newAccount)
                    : await _reader.CreateReaderAsync(newAccount);

                if (isCreated)
                {
                    Message.Text = "Tạo thành công";
                    await Task.Delay(1500);

                    if (DataContext is LoginViewModel vm)
                    {
                        vm.IsRegisterMode = false;
                    }

                    registerForm.Visibility = Visibility.Hidden;
                    loginForm.Visibility = Visibility.Visible;
                }
                else
                {
                    Message.Text = "Email hoặc số điện thoại đã được sử dụng.";
                }
            }

            catch (Exception ex)

            {
                Message.Text = $"Lỗi hệ thống: {ex.Message}";
                Console.WriteLine(ex.ToString());
            }
        }

        private async void ButtonLogin_Clicked(object sender, RoutedEventArgs e)
        {

            if (DataContext is LoginViewModel vm)
            {
                vm.ErrorMessage = string.Empty;
            }

            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string roleChoice = (string)loginRole.SelectedItem;


            if (string.IsNullOrEmpty(roleChoice))

            {
                if (DataContext is LoginViewModel vm2)
                {
                    vm2.ErrorMessage = "Vui lòng chọn vai trò.";
                }
                return;
            }

            try
            {
                Role role = null;
                if (roleChoice == "Author")
                {
                    role = await _role.GetByNameAsync("Author");
                }
                else if (roleChoice == "Reader")
                {
                    role = await _role.GetByNameAsync("Reader");
                }
                else if (roleChoice == "Admin")
                {
                    role = await _role.GetByNameAsync("Admin");
                }

                if (role == null)
                {
                    if (DataContext is LoginViewModel vm2)
                    {
                        vm2.ErrorMessage = "Không tìm thấy vai trò đã chọn.";
                    }
                    return;
                }

                Account accountDb = await _account.CheckLoginAsync(email, password, role.RoleId);
                if (accountDb != null)
                {
                    // Save authenticated user to static session
                    BookManagement.Services.Utils.UserSession.CurrentUser = accountDb;

                    string accessToken = await _token.GenerateAccessTokenAsync(accountDb.AccountId);
                    UserSecretContext sqliteContext = new UserSecretContext();
                    await sqliteContext.Database.EnsureCreatedAsync();
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

                    if (role.RoleName == "Author")
                    {
                        Services.Navigation.NavigationService.Instance.NavigateMain(new AuthorDashboard());
                    }
                    else if (role.RoleName == "Reader")
                    {
                        Services.Navigation.NavigationService.Instance.NavigateMain(new ReaderDashboard());
                    }
                    else if (role.RoleName == "Admin")
                    {
                        Services.Navigation.NavigationService.Instance.NavigateMain(new AdminDashboard());
                    }
                }
                else
                {
                    if (DataContext is LoginViewModel vm2)
                    {
                        vm2.ErrorMessage = "Email hoặc mật khẩu không chính xác.";
                    }
                }

            }
            catch (Exception ex)
            {

                if (DataContext is LoginViewModel vm2)
                {
                    vm2.ErrorMessage = $"Lỗi kết nối cơ sở dữ liệu: {ex.Message}";
                }
                Console.WriteLine(ex.ToString());
            }
        }

        private void Vm_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LoginViewModel.IsRegisterMode))
            {
                if (txtPassword != null) txtPassword.Password = string.Empty;
                if (txtRegisterPassword != null) txtRegisterPassword.Password = string.Empty;
                if (txtRegisterConfirmPassword != null) txtRegisterConfirmPassword.Password = string.Empty;
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
