using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookManagement.Models.Entities
{
    public class BookModel : INotifyPropertyChanged
    {
        private int _stock = 100;

        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = "Draft"; // Approved, Pending, Rejected, Draft
        public string CoverImagePath { get; set; } = string.Empty;
        public int Stock
        {
            get => _stock;
            set
            {
                if (_stock == value) return;
                _stock = value;
                OnPropertyChanged();
            }
        }
        public string Description { get; set; } = string.Empty;
        public string SubmittedDate { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }

        public Guid BookId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
