using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using BookManagement.Services.Mock;

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
            services.AddSingleton<IReaderService, MockReaderService>();
            services.AddSingleton<IUserService, BookManagement.Services.Repository.UserService>();
            services.AddSingleton<IPurchaseService, MockPurchaseService>();
            services.AddSingleton<IReviewService, MockReviewService>();

            // Register Core Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<ReaderDashboardViewModel>();
            services.AddTransient<AuthorDashboardViewModel>();
            services.AddTransient<AdminDashboardViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
