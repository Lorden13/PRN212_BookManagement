using System.Linq;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class AuthorDashboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public AuthorDashboardViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            var books = MockDataService.Books.Where(b => b.AuthorId == mainViewModel.CurrentUser?.Id).ToList();
            TotalBooks = books.Count;
            PendingCount = books.Count(b => b.Status == BookStatus.Pending);
            ApprovedCount = books.Count(b => b.Status == BookStatus.Approved);
            RejectedCount = books.Count(b => b.Status == BookStatus.Rejected);
        }

        public int TotalBooks { get; }
        public int PendingCount { get; }
        public int ApprovedCount { get; }
        public int RejectedCount { get; }
    }
}
