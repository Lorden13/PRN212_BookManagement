using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderHomeViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IPurchaseService _purchaseService;

        private string _searchText = string.Empty;
        private string _selectedCategory = "Tất cả thể loại";

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public List<string> Categories { get; } = new List<string>();

        public ObservableCollection<BookViewModel> PopularBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<BookViewModel> RecommendedBooks { get; } = new ObservableCollection<BookViewModel>();
        public ObservableCollection<RecentlyPurchasedItemViewModel> RecentPurchases { get; } = new ObservableCollection<RecentlyPurchasedItemViewModel>();

        public UserProfileModel CurrentUser => _dashboard.Sidebar.CurrentUser;

        public ICommand ViewDetailCommand { get; }
        public ICommand ExecuteSearchCommand { get; }
        public ICommand ViewAllBooksCommand { get; }
        public ICommand ViewAllLibraryCommand { get; }
        public ICommand ViewAllFavoritesCommand { get; }
        public ICommand ViewProfileCommand { get; }

        public ReaderHomeViewModel(DashboardViewModelBase dashboard, IBookService bookService, IPurchaseService purchaseService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _purchaseService = purchaseService;

            ViewDetailCommand = new RelayCommand<BookViewModel>(OnViewDetail);
            ExecuteSearchCommand = new RelayCommand(OnExecuteSearch);
            ViewAllBooksCommand = new RelayCommand(() => NavigateToMenuItem("Books"));
            ViewAllLibraryCommand = new RelayCommand(() => NavigateToMenuItem("My Library"));
            ViewAllFavoritesCommand = new RelayCommand(() => NavigateToMenuItem("Favorites"));
            ViewProfileCommand = new RelayCommand(() => NavigateToMenuItem("Profile"));

            // Populate categories
            Categories.Add("Tất cả thể loại");
            var approved = _bookService.GetApprovedBooks().ToList();
            foreach (var cat in approved.Select(b => b.Category).Distinct())
            {
                Categories.Add(cat);
            }

            LoadBooks();
            LoadRecentPurchases();
        }

        private void LoadBooks()
        {
            var approved = _bookService.GetApprovedBooks().ToList();

            // Popular Books (sorted by rating, first 4)
            foreach (var b in approved.OrderByDescending(x => x.Rating).Take(4))
            {
                PopularBooks.Add(new BookViewModel(b) { SelectCommand = ViewDetailCommand });
            }

            // Recommended Books (first 4 of novel or random category)
            foreach (var b in approved.Skip(4).Take(4))
            {
                RecommendedBooks.Add(new BookViewModel(b) { SelectCommand = ViewDetailCommand });
            }
        }

        private void LoadRecentPurchases()
        {
            RecentPurchases.Clear();
            var history = _purchaseService.GetPurchaseHistory(1)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(3);

            foreach (var p in history)
            {
                var book = _bookService.GetBookById(p.BookId);
                RecentPurchases.Add(new RecentlyPurchasedItemViewModel
                {
                    BookId = p.BookId,
                    Title = p.BookTitle,
                    CoverImagePath = book?.CoverImagePath ?? string.Empty,
                    PurchaseDate = p.PurchaseDate,
                    Price = p.Price,
                    Status = p.Status
                });
            }
        }

        private void OnViewDetail(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.CurrentPageViewModel = new ReaderBookDetailViewModel(_dashboard, _bookService, _purchaseService, book);
            }
        }

        private void OnExecuteSearch()
        {
            // Navigate to Books page with search filters
            var booksVM = new ReaderBooksViewModel(_dashboard, _bookService, _purchaseService)
            {
                SearchText = this.SearchText,
                SelectedCategory = this.SelectedCategory == "Tất cả thể loại" ? "Tất cả thể loại" : this.SelectedCategory
            };

            // Set current page view model and highlight menu item
            _dashboard.PageTitle = "Books";
            _dashboard.CurrentPageViewModel = booksVM;

            var menuItem = _dashboard.Sidebar.MenuItems.FirstOrDefault(m => m.Title == "Books");
            if (menuItem != null)
            {
                // Set SelectedItem without triggering Command execute (or let it execute but we already set the VM)
                // Deselect others
                foreach (var item in _dashboard.Sidebar.MenuItems)
                {
                    item.IsSelected = (item == menuItem);
                }
                _dashboard.Sidebar.SelectedItem = menuItem;
            }
        }

        private void NavigateToMenuItem(string title)
        {
            var item = _dashboard.Sidebar.MenuItems.FirstOrDefault(m => m.Title == title);
            if (item != null)
            {
                _dashboard.Sidebar.SelectedItem = item;
            }
        }
    }

    public class RecentlyPurchasedItemViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CoverImagePath { get; set; } = string.Empty;
        public string PurchaseDate { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = "Completed";
    }
}
