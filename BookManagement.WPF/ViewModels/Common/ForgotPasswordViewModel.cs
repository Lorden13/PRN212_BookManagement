using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services.Navigation;

namespace BookManagement.ViewModels.Common
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private int _currentStep = 1;
        private string _email = string.Empty;
        private string _otp = string.Empty;
        private string _newPassword = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isNewPasswordVisible;
        private bool _isConfirmPasswordVisible;

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (SetProperty(ref _currentStep, value))
                {
                    ErrorMessage = string.Empty;
                    SuccessMessage = string.Empty;
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string Otp
        {
            get => _otp;
            set
            {
                if (SetProperty(ref _otp, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                {
                    ErrorMessage = string.Empty;
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public bool IsNewPasswordVisible
        {
            get => _isNewPasswordVisible;
            set => SetProperty(ref _isNewPasswordVisible, value);
        }

        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set => SetProperty(ref _isConfirmPasswordVisible, value);
        }

        public ICommand SendOtpCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand BackToLoginCommand { get; }
        public ICommand ToggleNewPasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }

        public ForgotPasswordViewModel()
        {
            SendOtpCommand = new RelayCommand(ExecuteSendOtp, CanSendOtp);
            ResetPasswordCommand = new RelayCommand(ExecuteResetPassword, CanResetPassword);
            BackToLoginCommand = new RelayCommand(ExecuteBackToLogin);
            ToggleNewPasswordVisibilityCommand = new RelayCommand(() => IsNewPasswordVisible = !IsNewPasswordVisible);
            ToggleConfirmPasswordVisibilityCommand = new RelayCommand(() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible);
        }

        private bool CanSendOtp()
        {
            return !string.IsNullOrWhiteSpace(Email);
        }

        private void ExecuteSendOtp()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (!IsValidEmail(Email))
            {
                ErrorMessage = "Email không hợp lệ. Vui lòng nhập đúng định dạng.";
                return;
            }

            // Simulate sending OTP
            SuccessMessage = $"Mã OTP đã được gửi đến {Email}!";
            CurrentStep = 2;
        }

        private bool CanResetPassword()
        {
            return !string.IsNullOrWhiteSpace(Otp) &&
                   !string.IsNullOrWhiteSpace(NewPassword) &&
                   !string.IsNullOrWhiteSpace(ConfirmPassword);
        }

        private void ExecuteResetPassword()
        {
            ErrorMessage = string.Empty;

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có tối thiểu 6 ký tự.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return;
            }

            // Simulate reset password success
            System.Windows.MessageBox.Show("Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            ExecuteBackToLogin();
        }

        private void ExecuteBackToLogin()
        {
            NavigationService.Instance.NavigateMain(new Views.Common.Login());
        }

        private bool IsValidEmail(string email)
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
    }
}
