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
            services.AddSingleton<IBookService, BookManagement.Services.Repository.BookService>();
            services.AddSingleton<IAuthorService, BookManagement.Services.Repository.AuthorService>();
            services.AddSingleton<IReaderService, BookManagement.Services.Repository.ReaderService>();
            services.AddSingleton<IUserService, BookManagement.WPF.Services.Transactions.UserService>();
            services.AddSingleton<IReviewService, BookManagement.WPF.Services.Transactions.ReviewService>();
            services.AddTransient<BookManagement.WPF.Entities.ProjectPrnContext>();
            services.AddTransient<BookManagement.WPF.Services.Transactions.IApprovalService, BookManagement.WPF.Services.Transactions.ApprovalService>();
            services.AddTransient<BookManagement.WPF.Services.Transactions.IPurchaseTransactionService, BookManagement.WPF.Services.Transactions.PurchaseTransactionService>();
            services.AddTransient<BookManagement.WPF.Services.Transactions.IFavoriteService, BookManagement.WPF.Services.Transactions.FavoriteService>();


            // Register Core Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IDialogService, DialogService>();

          

            return services.BuildServiceProvider();
        }
    }
}
