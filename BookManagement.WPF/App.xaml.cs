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

            // Register Mock Services


            // Register Core Services
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IDialogService, DialogService>();

            // Register ViewModels
      

            return services.BuildServiceProvider();
        }
    }
}
