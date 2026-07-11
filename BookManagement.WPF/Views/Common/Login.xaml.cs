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
            try
            {
                string fullFullname = registerFullname.Text;
                string email = registerEmail.Text;
                string address = registerAddress.Text;
                string password = txtRegisterPassword.Password;
                string confirmPassword = txtRegisterConfirmPasswordPlain.Text;
                string phone = registerPhone.Text;
                string role = (string)registerRole.SelectedItem;

                if (string.IsNullOrEmpty(role))
                {
                    Message.Text = "Vui lòng chọn vai trò.";
                    return;
                }

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
                            await Task.Delay(1500);

                            if (DataContext is LoginViewModel vm)
                            {
                                vm.IsRegisterMode = false;
                            }
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
                            await Task.Delay(1500);

                            if (DataContext is LoginViewModel vm)
                            {
                                vm.IsRegisterMode = false;
                            }
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
