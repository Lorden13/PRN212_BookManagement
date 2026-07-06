using System.Linq;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public AdminDashboardViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            var users = MockDataService.Users;
            var books = MockDataService.Books;
            TotalUsers = users.Count;
            TotalAuthors = users.Count(u => u.Role == UserRole.Author);
            TotalReaders = users.Count(u => u.Role == UserRole.Reader);
            TotalBooks = books.Count;
            PendingBooks = books.Count(b => b.Status == BookStatus.Pending);
        }

        public int TotalUsers { get; }
        public int TotalAuthors { get; }
        public int TotalReaders { get; }
        public int TotalBooks { get; }
        public int PendingBooks { get; }
    }
}
