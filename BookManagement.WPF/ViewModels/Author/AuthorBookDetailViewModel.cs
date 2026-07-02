using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Author
{
    public class AuthorBookDetailViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IBookService _bookService;
        private readonly IReviewService? _reviewService;

        public BookViewModel Book { get; }
        public ObservableCollection<ReviewModel> Reviews { get; } = new ObservableCollection<ReviewModel>();

        public ICommand BackCommand { get; }
        public ICommand EditCommand { get; }

        public AuthorBookDetailViewModel(DashboardViewModelBase dashboard, IBookService bookService, BookViewModel book)
        {
            _dashboard = dashboard;
            _bookService = bookService;
            _reviewService = App.Current.Services.GetService(typeof(IReviewService)) as IReviewService;
            Book = book;

            BackCommand = new RelayCommand(OnBack);
            EditCommand = new RelayCommand(OnEdit, CanEdit);

            LoadReviews();
        }

        private void LoadReviews()
        {
            Reviews.Clear();
            if (_reviewService != null)
            {
                var bookReviews = _reviewService.GetReviewsByBookId(Book.Id);
                foreach (var r in bookReviews)
                {
                    Reviews.Add(r);
                }
            }
        }

        private void OnBack()
        {
            _dashboard.PageTitle = "My Books";
            _dashboard.CurrentPageViewModel = new AuthorBooksViewModel(_dashboard, _bookService);
        }

        private bool CanEdit()
        {
            return Book != null && Book.Status != "Approved";
        }

        private void OnEdit()
        {
            if (Book != null)
            {
                _dashboard.PageTitle = "Edit Book";
                _dashboard.CurrentPageViewModel = new AuthorCreateBookViewModel(_dashboard, _bookService, Book);
            }
        }
    }
}
