using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;
using BookManagement.Services.Mock;

namespace BookManagement.ViewModels.Admin
{
    public class AdminReaderDetailViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IReaderService _readerService;
        private readonly IPurchaseService _purchaseService;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;

        public ReaderModel Reader { get; }

        public int TotalPurchases { get; }
        public double TotalSpending { get; }

        public ObservableCollection<PurchaseModel> PurchaseHistory { get; } = new ObservableCollection<PurchaseModel>();
        public ObservableCollection<BookViewModel> FavoriteBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<ActivityLogModel> Activities { get; } = new ObservableCollection<ActivityLogModel>();

        public ICommand BackCommand { get; }

        public AdminReaderDetailViewModel(
            DashboardViewModelBase dashboard,
            IReaderService readerService,
            IPurchaseService purchaseService,
            IBookService bookService,
            IAuthorService authorService,
            ReaderModel reader)
        {
            _dashboard = dashboard;
            _readerService = readerService;
            _purchaseService = purchaseService;
            _bookService = bookService;
            _authorService = authorService;
            Reader = reader;

            BackCommand = new RelayCommand(OnBack);

            // Purchase History
            var readerPurchases = _purchaseService.GetPurchaseHistory(Reader.Id).ToList();
            TotalPurchases = readerPurchases.Count(p => p.Status == "Completed");
            TotalSpending = readerPurchases.Where(p => p.Status == "Completed").Sum(p => p.Price);

            foreach (var p in readerPurchases)
            {
                PurchaseHistory.Add(p);
            }

            // Favorite Books
            var favoriteIds = MockReaderService.FavoriteBookIds;
            foreach (var id in favoriteIds)
            {
                var book = _bookService.GetBookById(id);
                if (book != null)
                {
                    FavoriteBooks.Add(new BookViewModel(book));
                }
            }

            // Activities Timeline (Mocked for detailed logs)
            Activities.Add(new ActivityLogModel { Message = "Đăng nhập hệ thống", Time = "Hôm nay, 08:30 AM", Type = "Login" });
            if (PurchaseHistory.Count > 0)
            {
                var lastPurchase = PurchaseHistory.First();
                Activities.Add(new ActivityLogModel { Message = $"Đã mua sách '{lastPurchase.BookTitle}'", Time = lastPurchase.PurchaseDate, Type = "Purchase" });
            }
            Activities.Add(new ActivityLogModel { Message = "Đã thêm 'Clean Code' vào danh sách yêu thích", Time = "2026-06-24", Type = "FavoriteAdd" });
            Activities.Add(new ActivityLogModel { Message = "Đã xóa 'Java Basics' khỏi danh sách yêu thích", Time = "2026-06-20", Type = "FavoriteRemove" });
        }

        private void OnBack()
        {
            _dashboard.PageTitle = "User Management";
            _dashboard.CurrentPageViewModel = new AdminUsersViewModel(_dashboard, _readerService, _authorService, _purchaseService, _bookService);
        }
    }
}
