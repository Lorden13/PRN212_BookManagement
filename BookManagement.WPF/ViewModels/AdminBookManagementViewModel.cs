using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class AdminBookManagementViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _searchText = string.Empty;
        private string _selectedStatus = "All";

        public AdminBookManagementViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            StatusOptions = new List<string> { "All", "Pending", "Approved", "Rejected" };

            SearchCommand = new RelayCommand(_ => ApplyFilter());
            ViewCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    MessageBox.Show($"Book Details:\n\nTitle: {book.Title}\nAuthor: {book.AuthorName}\nCategory: {book.Category}\nPrice: ${book.Price:F2}\nStatus: {book.Status}\nCreated: {book.CreatedDate:d}\n\n{book.Description}", "Book Details", MessageBoxButton.OK, MessageBoxImage.Information);
            });
            DeleteCommand = new RelayCommand(param =>
            {
                if (param is Book book)
                    MessageBox.Show($"Are you sure you want to delete \"{book.Title}\"?\n\n(Demo only — no data will be changed)", "Confirm Delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            });

            LoadBooks();
        }

        public ObservableCollection<Book> Books { get; } = new();
        public List<string> StatusOptions { get; }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilter();
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                    ApplyFilter();
            }
        }

        public bool HasBooks => Books.Count > 0;

        public RelayCommand SearchCommand { get; }
        public RelayCommand ViewCommand { get; }
        public RelayCommand DeleteCommand { get; }

        private void LoadBooks()
        {
            Books.Clear();
            foreach (var book in MockDataService.Books)
                Books.Add(book);
            OnPropertyChanged(nameof(HasBooks));
        }

        private void ApplyFilter()
        {
            Books.Clear();
            var filtered = MockDataService.Books.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.Trim();
                filtered = filtered.Where(b =>
                    b.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedStatus != "All")
            {
                if (Enum.TryParse<BookStatus>(SelectedStatus, out var status))
                    filtered = filtered.Where(b => b.Status == status);
            }

            foreach (var book in filtered)
                Books.Add(book);

            OnPropertyChanged(nameof(HasBooks));
        }
    }
}
