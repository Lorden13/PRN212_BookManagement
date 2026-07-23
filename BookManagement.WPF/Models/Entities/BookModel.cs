using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookManagement.Models.Entities
{
    public class BookModel : INotifyPropertyChanged
    {
        private double _rating;

        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Status { get; set; } = "Draft"; // Approved, Pending, Rejected, Draft
        public string CoverImagePath { get; set; } = string.Empty;
        public double Rating
        {
            get => _rating;
            set
            {
                if (Math.Abs(_rating - value) < double.Epsilon) return;
                _rating = value;
                OnPropertyChanged();
            }
        }
        public string Description { get; set; } = string.Empty;
        public string SubmittedDate { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        private int _stock = 10;
        public int Stock
        {
            get => _stock;
            set
            {
                if (_stock == value) return;
                _stock = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StockDisplay));
                OnPropertyChanged(nameof(IsInStock));
            }
        }

        public string StockDisplay => Stock > 0 ? $"Còn {Stock} cuốn" : "Hết hàng";
        public bool IsInStock => Stock > 0;

        public Guid BookId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
