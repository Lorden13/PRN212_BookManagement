
namespace BookManagement.ViewModels.Common
{
    public class BookViewModel : BaseViewModel
    {
        private readonly BookModel _model;

        public BookViewModel(BookModel model)
        {
            _model = model;
        }

        public BookModel Model => _model;

        public int Id => _model.Id;

        public string Title
        {
            get => _model.Title;
            set
            {
                if (_model.Title != value)
                {
                    _model.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Author
        {
            get => _model.Author;
            set
            {
                if (_model.Author != value)
                {
                    _model.Author = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Category
        {
            get => _model.Category;
            set
            {
                if (_model.Category != value)
                {
                    _model.Category = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Price
        {
            get => _model.Price;
            set
            {
                if (_model.Price != value)
                {
                    _model.Price = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => _model.Status;
            set
            {
                if (_model.Status != value)
                {
                    _model.Status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CoverImagePath
        {
            get => _model.CoverImagePath;
            set
            {
                if (_model.CoverImagePath != value)
                {
                    _model.CoverImagePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PdfFilePath
        {
            get => _model.PdfFilePath;
            set
            {
                if (_model.PdfFilePath != value)
                {
                    _model.PdfFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

        public double Rating
        {
            get => _model.Rating;
            set
            {
                if (_model.Rating != value)
                {
                    _model.Rating = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _model.Description;
            set
            {
                if (_model.Description != value)
                {
                    _model.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SubmittedDate
        {
            get => _model.SubmittedDate;
            set
            {
                if (_model.SubmittedDate != value)
                {
                    _model.SubmittedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private System.Windows.Input.ICommand? _buyCommand;
        public System.Windows.Input.ICommand? BuyCommand
        {
            get => _buyCommand;
            set => SetProperty(ref _buyCommand, value);
        }

        private System.Windows.Input.ICommand? _favoriteCommand;
        public System.Windows.Input.ICommand? FavoriteCommand
        {
            get => _favoriteCommand;
            set => SetProperty(ref _favoriteCommand, value);
        }

        private System.Windows.Input.ICommand? _selectCommand;
        public System.Windows.Input.ICommand? SelectCommand
        {
            get => _selectCommand;
            set => SetProperty(ref _selectCommand, value);
        }
    }
}
