using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Reader
{
    public class ReaderBooksViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IPurchaseService _purchaseService;

        private string _searchText = string.Empty;
        private string _selectedCategory = "Tất cả thể loại";
        private string _selectedSortOption = "Mới nhất";
        private double _maxPrice = 50.0;
        private string _selectedRatingOption = "Tất cả đánh giá";
        
        private int _currentPage = 1;
        private int _pageSize = 8;
        private int _totalPages = 1;
        private int _totalItems = 0;
        private string _paginationInfo = "Showing 0 to 0 of 0 books";
        private string _pageInfoText = "Page 1 of 1";

        public ObservableCollection<BookViewModel> PagedBooks { get; } = new ObservableCollection<BookViewModel>();
        private readonly List<BookModel> _allBooks = new List<BookModel>();

        public List<string> Categories { get; } = new List<string>();
        public List<string> SortOptions { get; } = new List<string>
        {
            "Mới nhất",
            "Đánh giá cao nhất",
            "Giá thấp đến cao",
            "Giá cao đến thấp",
            "Tên sách A-Z"
        };

        public List<string> RatingOptions { get; } = new List<string>
        {
            "Tất cả đánh giá",
            "4.8 sao trở lên",
            "4.5 sao trở lên",
            "4.0 sao trở lên",
            "3.0 sao trở lên"
        };

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    CurrentPage = 1;
                    ApplyFiltersAndPaging();
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    CurrentPage = 1;
                    ApplyFiltersAndPaging();
                }
            }
        }

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (SetProperty(ref _selectedSortOption, value))
                {
                    ApplyFiltersAndPaging();
                }
            }
        }

        public double MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (SetProperty(ref _maxPrice, value))
                {
                    CurrentPage = 1;
                    ApplyFiltersAndPaging();
                }
            }
        }

        public string SelectedRatingOption
        {
            get => _selectedRatingOption;
            set
            {
                if (SetProperty(ref _selectedRatingOption, value))
                {
                    CurrentPage = 1;
                    ApplyFiltersAndPaging();
                }
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public string PaginationInfo
        {
            get => _paginationInfo;
            set => SetProperty(ref _paginationInfo, value);
        }

        public string PageInfoText
        {
            get => _pageInfoText;
            set => SetProperty(ref _pageInfoText, value);
        }

        public ICommand ViewDetailCommand { get; }
        public ICommand BuyCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        public ReaderBooksViewModel(DashboardViewModelBase dashboard, IBookService bookService, IPurchaseService purchaseService)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _purchaseService = purchaseService;

            ViewDetailCommand = new RelayCommand<BookViewModel>(OnViewDetail);
            BuyCommand = new RelayCommand<BookViewModel>(OnBuyBook);
            NextPageCommand = new RelayCommand(OnNextPage);
            PrevPageCommand = new RelayCommand(OnPrevPage);
            ClearFiltersCommand = new RelayCommand(OnClearFilters);

            // Populate categories list
            Categories.Add("Tất cả thể loại");
            var approved = _bookService.GetApprovedBooks().ToList();
            _allBooks.AddRange(approved);

            foreach (var cat in approved.Select(b => b.Category).Distinct())
            {
                Categories.Add(cat);
            }

            ApplyFiltersAndPaging();
        }

        private void ApplyFiltersAndPaging()
        {
            // 1. Filtering
            var filtered = _allBooks.AsEnumerable();

            // Text search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var text = SearchText.ToLower();
                filtered = filtered.Where(b => b.Title.ToLower().Contains(text) || b.Author.ToLower().Contains(text));
            }

            // Category dropdown
            if (SelectedCategory != "Tất cả thể loại" && !string.IsNullOrEmpty(SelectedCategory))
            {
                filtered = filtered.Where(b => b.Category == SelectedCategory);
            }

            // Max price filter
            filtered = filtered.Where(b => b.Price <= MaxPrice);

            // Rating filter
            double minRating = 0.0;
            if (SelectedRatingOption == "4.8 sao trở lên") minRating = 4.8;
            else if (SelectedRatingOption == "4.5 sao trở lên") minRating = 4.5;
            else if (SelectedRatingOption == "4.0 sao trở lên") minRating = 4.0;
            else if (SelectedRatingOption == "3.0 sao trở lên") minRating = 3.0;

            if (minRating > 0.0)
            {
                filtered = filtered.Where(b => b.Rating >= minRating);
            }

            // 2. Sorting
            switch (SelectedSortOption)
            {
                case "Mới nhất":
                    filtered = filtered.OrderByDescending(b => b.Id);
                    break;
                case "Đánh giá cao nhất":
                    filtered = filtered.OrderByDescending(b => b.Rating);
                    break;
                case "Giá thấp đến cao":
                    filtered = filtered.OrderBy(b => b.Price);
                    break;
                case "Giá cao đến thấp":
                    filtered = filtered.OrderByDescending(b => b.Price);
                    break;
                case "Tên sách A-Z":
                    filtered = filtered.OrderBy(b => b.Title);
                    break;
            }

            // 3. Pagination calculations
            var list = filtered.ToList();
            TotalItems = list.Count;
            TotalPages = (int)Math.Ceiling((double)TotalItems / _pageSize);
            if (TotalPages < 1) TotalPages = 1;

            if (CurrentPage > TotalPages) CurrentPage = TotalPages;
            if (CurrentPage < 1) CurrentPage = 1;

            // 4. Slicing
            var pageItems = list.Skip((CurrentPage - 1) * _pageSize).Take(_pageSize);

            PagedBooks.Clear();
            foreach (var b in pageItems)
            {
                PagedBooks.Add(new BookViewModel(b)
                {
                    BuyCommand = this.BuyCommand,
                    SelectCommand = this.ViewDetailCommand
                });
            }

            // Update info strings
            int startItem = TotalItems == 0 ? 0 : (CurrentPage - 1) * _pageSize + 1;
            int endItem = Math.Min(CurrentPage * _pageSize, TotalItems);
            
            PaginationInfo = $"Showing {startItem} to {endItem} of {TotalItems} books";
            PageInfoText = $"Page {CurrentPage} of {TotalPages}";
        }

        private void OnNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                ApplyFiltersAndPaging();
            }
        }

        private void OnPrevPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                ApplyFiltersAndPaging();
            }
        }

        private void OnClearFilters()
        {
            _searchText = string.Empty;
            _selectedCategory = "Tất cả thể loại";
            _selectedSortOption = "Mới nhất";
            _maxPrice = 50.0;
            _selectedRatingOption = "Tất cả đánh giá";
            _currentPage = 1;
            
            OnPropertyChanged(nameof(SearchText));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(SelectedSortOption));
            OnPropertyChanged(nameof(MaxPrice));
            OnPropertyChanged(nameof(SelectedRatingOption));
            OnPropertyChanged(nameof(CurrentPage));

            ApplyFiltersAndPaging();
        }

        private void OnViewDetail(BookViewModel book)
        {
            if (book != null)
            {
                _dashboard.CurrentPageViewModel = new ReaderBookDetailViewModel(_dashboard, _bookService, _purchaseService, book);
            }
        }

        private void OnBuyBook(BookViewModel book)
        {
            if (book != null)
            {
                _purchaseService.PurchaseBook(1, book.Id);
                _dashboard.ShowToast($"Đã mua thành công sách: {book.Title}!", "Success");
            }
        }
    }
}
