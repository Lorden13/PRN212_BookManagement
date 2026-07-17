using Microsoft.Extensions.DependencyInjection;

namespace BookManagement
{
    public partial class App : Application
    {
        public IServiceProvider Services { get; private set; }

        public new static App Current => (App)Application.Current;

        public App()
        {
            Services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();


            // Register Services
            services.AddSingleton<IBookService, BookService>();
            services.AddSingleton<IAuthorService, BookManagement.Services.Repository.AuthorService>();
            services.AddSingleton<IReaderService, BookManagement.Services.Repository.ReaderService>();
            services.AddSingleton<IUserService, BookManagement.Services.Repository.UserService>();
            services.AddSingleton<IPurchaseService, MockPurchaseService>();
            services.AddSingleton<IReviewService, MockReviewService>();

            // Backend Developer 3 services. These use EF Core and SQL Server;
            // the existing frontend contracts above remain untouched.
            services.AddSingleton<BookManagement.WPF.Services.Transactions.IApprovalService>(
                _ => new BookManagement.WPF.Services.Transactions.ApprovalService());
            services.AddSingleton<BookManagement.WPF.Services.Transactions.IPurchaseTransactionService>(
                _ => new BookManagement.WPF.Services.Transactions.PurchaseTransactionService());
            services.AddSingleton<BookManagement.WPF.Services.Transactions.IFavoriteService>(
                _ => new BookManagement.WPF.Services.Transactions.FavoriteService());


            // Register Core Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register ViewModels
            services.AddTransient<BookManagement.ViewModels.Common.LoginViewModel>();
            services.AddTransient<BookManagement.ViewModels.Common.ForgotPasswordViewModel>();
            services.AddTransient<BookManagement.ViewModels.Reader.ReaderDashboardViewModel>();
            services.AddTransient<BookManagement.ViewModels.Author.AuthorDashboardViewModel>();
            services.AddTransient<BookManagement.ViewModels.Admin.AdminDashboardViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
