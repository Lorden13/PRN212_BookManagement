using System.Collections.ObjectModel;
using System.Linq;

namespace BookManagement.ViewModels.Admin
{
    public class AdminReportsViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IPurchaseService _purchaseService;
        private readonly IReaderService _readerService;
        private readonly IAuthorService _authorService;

        public double TotalRevenue { get; }
        public int TotalBooksSold { get; }
        public int TotalReaders { get; }
        public int TotalAuthors { get; }

        public ObservableCollection<TopItemModel> TopCategories { get; } = new ObservableCollection<TopItemModel>();
        public ObservableCollection<TopItemModel> TopReaders { get; } = new ObservableCollection<TopItemModel>();
        public ObservableCollection<TopItemModel> TopAuthors { get; } = new ObservableCollection<TopItemModel>();
        public ObservableCollection<PurchaseModel> RecentPurchases { get; } = new ObservableCollection<PurchaseModel>();

        public AdminReportsViewModel(
            DashboardViewModelBase dashboard,
            IPurchaseService purchaseService,
            IReaderService readerService,
            IAuthorService authorService)
        {
            _dashboard = dashboard;
            _purchaseService = purchaseService;
            _readerService = readerService;
            _authorService = authorService;

            var purchases = _purchaseService.GetAllPurchases().ToList();
            var readers = _readerService.GetAllReaders().ToList();
            var authors = _authorService.GetAllAuthors().ToList();

            TotalRevenue = purchases.Where(p => p.Status == "Completed").Sum(p => p.Price);
            TotalBooksSold = purchases.Count(p => p.Status == "Completed");
            TotalReaders = readers.Count;
            TotalAuthors = authors.Count;

            // Seed Top Categories
            TopCategories.Add(new TopItemModel { Name = "Programming", Metric = "12 cuốn", Percentage = 40 });
            TopCategories.Add(new TopItemModel { Name = "Novel", Metric = "8 cuốn", Percentage = 27 });
            TopCategories.Add(new TopItemModel { Name = "Business", Metric = "5 cuốn", Percentage = 17 });
            TopCategories.Add(new TopItemModel { Name = "Science", Metric = "3 cuốn", Percentage = 10 });
            TopCategories.Add(new TopItemModel { Name = "History", Metric = "2 cuốn", Percentage = 6 });

            // Seed Top Readers
            var groupedReaders = purchases
                .Where(p => p.Status == "Completed")
                .GroupBy(p => p.ReaderName)
                .Select(g => new { Name = g.Key, Count = g.Count(), Total = g.Sum(x => x.Price) })
                .OrderByDescending(x => x.Count)
                .Take(5);

            foreach (var r in groupedReaders)
            {
                TopReaders.Add(new TopItemModel { Name = r.Name, Metric = $"{r.Count} cuốn (${r.Total:F2})", Percentage = 100 });
            }

            // Seed Top Authors
            TopAuthors.Add(new TopItemModel { Name = "Robert C. Martin", Metric = "15 cuốn ($149.85)", Percentage = 100 });
            TopAuthors.Add(new TopItemModel { Name = "Martin Fowler", Metric = "8 cuốn ($91.92)", Percentage = 70 });
            TopAuthors.Add(new TopItemModel { Name = "J. K. Rowling", Metric = "5 cuốn ($44.95)", Percentage = 50 });
            TopAuthors.Add(new TopItemModel { Name = "Carl Sagan", Metric = "2 cuốn ($21.00)", Percentage = 20 });

            // Seed Recent Purchases
            foreach (var p in purchases.OrderByDescending(p => p.PurchaseDate).Take(5))
            {
                RecentPurchases.Add(p);
            }
        }
    }
}
