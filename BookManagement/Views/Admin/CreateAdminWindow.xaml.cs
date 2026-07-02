using BookManagement.Services.AccountService;
using BookManagement.Services.AdminService;

namespace BookManagement.Views.Admin
{
    /// <summary>
    /// Interaction logic for CreateAdminWindow.xaml
    /// </summary>
    public partial class CreateAdminWindow : Window
    {
        private readonly AccountService _account;
        private readonly AdminService _admin;
       
        public CreateAdminWindow()
        {

            InitializeComponent();
            _account = new AccountService();
            _admin = new AdminService();
        }

        private async void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string password = Password.Password;
            string fullname = Fullname.Text;
            string email = Email.Text;
            string phone = Phone.Text;
            string address = Address.Text;
            ///tu check validate
            bool isCreated = await _admin.CreateAdminAsync(new Entities.Account()
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
