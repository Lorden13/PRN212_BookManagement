using BookManagement.Models.Entities;
using BookManagement.Services.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using NavigationService =
BookManagement.Services.Navigation.NavigationService;
namespace BookManagement.Views.Reader
{
    public partial class ReaderBooksView : UserControl
    {
        private readonly IBookService _bookService;
        private List<BookModel> _allBooks = new List<BookModel>();
        private List<BookModel> _filteredBooks = new List<BookModel>();

        private int _currentPage = 1;
        private const int PageSize = 6;
        private bool _isInitializing = true;

        public ReaderBooksView() : this(string.Empty, string.Empty)
        {
        }

        public ReaderBooksView(string searchQuery, string category)
        {
            InitializeComponent();

            _bookService = App.Current.Services.GetRequiredService<IBookService>();

            Loaded += (s, e) =>
            {
                InitializeFilters(searchQuery, category);
                LoadData();
                _isInitializing = false;
                ApplyFiltersAndPagination();
            };
        }

        private void InitializeFilters(string searchQuery, string category)
        {
            // Load Books to scan categories
            _allBooks = _bookService.GetApprovedBooks().ToList();

            // Populate Category ComboBox
            var categories = new List<string> { "Tất cả" };
            categories.AddRange(_allBooks.Select(b => b.Category).Distinct());
            cbCategory.ItemsSource = categories;

            // Select default or passed category
            if (!string.IsNullOrEmpty(category) && categories.Contains(category))
            {
                cbCategory.SelectedItem = category;
            }
            else
            {
                cbCategory.SelectedIndex = 0;
            }

            // Populate Rating ComboBox
            cbRating.ItemsSource = new List<string> { "Tất cả đánh giá", "3.0+ ★", "4.0+ ★", "4.5+ ★" };
            cbRating.SelectedIndex = 0;

            // Populate Sort ComboBox
            cbSort.ItemsSource = new List<string> { "Sắp xếp mặc định", "Tên: A-Z", "Tên: Z-A", "Giá: Thấp đến Cao", "Giá: Cao đến Thấp" };
            cbSort.SelectedIndex = 0;

            // Set search query if passed
            if (!string.IsNullOrEmpty(searchQuery))
            {
                txtSearch.Text = searchQuery;
            }
        }

        private void LoadData()
        {
            _allBooks = _bookService.GetApprovedBooks().ToList();
        }

        private void FilterInput_Changed(object sender, EventArgs e)
        {
            if (_isInitializing) return;
            _currentPage = 1;
            ApplyFiltersAndPagination();
        }

        private void ApplyFiltersAndPagination()
        {
            string query = txtSearch.Text.Trim().ToLower();
            string category = cbCategory.SelectedItem?.ToString() ?? "Tất cả";
            string ratingOpt = cbRating.SelectedItem?.ToString() ?? "Tất cả đánh giá";
            string sortOpt = cbSort.SelectedItem?.ToString() ?? "Sắp xếp mặc định";

            double maxPrice = double.MaxValue;
            if (!string.IsNullOrWhiteSpace(txtMaxPrice.Text) && double.TryParse(txtMaxPrice.Text, out double parsedPrice))
            {
                maxPrice = parsedPrice;
            }

            // 1. Filter
            var filtered = _allBooks.Where(b =>
            {
                bool matchQuery = string.IsNullOrEmpty(query) || 
                                  b.Title.ToLower().Contains(query) || 
                                  b.Author.ToLower().Contains(query) || 
                                  b.Category.ToLower().Contains(query);

                bool matchCategory = category == "Tất cả" || b.Category.Equals(category, StringComparison.OrdinalIgnoreCase);

                bool matchPrice = b.Price <= maxPrice;

                bool matchRating = true;
                if (ratingOpt == "3.0+ ★") matchRating = b.Rating >= 3.0;
                else if (ratingOpt == "4.0+ ★") matchRating = b.Rating >= 4.0;
                else if (ratingOpt == "4.5+ ★") matchRating = b.Rating >= 4.5;

                return matchQuery && matchCategory && matchPrice && matchRating;
            }).ToList();

            // 2. Sort
            if (sortOpt == "Tên: A-Z")
            {
                filtered = filtered.OrderBy(b => b.Title).ToList();
            }
            else if (sortOpt == "Tên: Z-A")
            {
                filtered = filtered.OrderByDescending(b => b.Title).ToList();
            }
            else if (sortOpt == "Giá: Thấp đến Cao")
            {
                filtered = filtered.OrderBy(b => b.Price).ToList();
            }
            else if (sortOpt == "Giá: Cao đến Thấp")
            {
                filtered = filtered.OrderByDescending(b => b.Price).ToList();
            }

            _filteredBooks = filtered;

            // 3. Paginate
            int totalItems = _filteredBooks.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            if (totalPages == 0) totalPages = 1;

            if (_currentPage > totalPages) _currentPage = totalPages;
            if (_currentPage < 1) _currentPage = 1;

            var paged = _filteredBooks.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();

            icBooks.ItemsSource = paged;

            // 4. Update UI
            emptyState.Visibility = totalItems == 0 ? Visibility.Visible : Visibility.Collapsed;

            int startItem = totalItems == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int endItem = Math.Min(_currentPage * PageSize, totalItems);

            pagination.InfoText = $"Showing {startItem} to {endItem} of {totalItems} books";
            pagination.PageInfoText = $"Page {_currentPage} of {totalPages}";
        }

        private void BtnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            _isInitializing = true;
            txtSearch.Text = string.Empty;
            cbCategory.SelectedIndex = 0;
            cbRating.SelectedIndex = 0;
            cbSort.SelectedIndex = 0;
            txtMaxPrice.Text = string.Empty;
            _currentPage = 1;
            _isInitializing = false;
            ApplyFiltersAndPagination();
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)_filteredBooks.Count / PageSize);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void BookCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (BookManagement.Controls.BookCard.IsActionButtonClick(e)) return;
            if (sender is FrameworkElement card && card.DataContext is BookModel clickedBook)
            {
                var detailView = new ReaderBookDetailView(clickedBook);
                var nav = NavigationService.GetNavigationService();
                if (nav != null)
                {
                    nav.NavigateContent(detailView);
                }
            }
        }
    }
}
