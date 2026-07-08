using System.Collections.Generic;
using System.Windows;
using BookManagement.WPF.Commands;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class EditBookViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _priceText = string.Empty;
        private string _selectedCategory = string.Empty;
        private string _pdfPath = string.Empty;

        private string _titleError = string.Empty;
        private string _categoryError = string.Empty;
        private string _priceError = string.Empty;
        private string _descriptionError = string.Empty;
        private string _pdfError = string.Empty;

        public EditBookViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Categories = new List<string>
            {
                "Technology", "Fiction", "Business", "Poetry",
                "History", "Lifestyle", "Self-Help", "Science Fiction"
            };
            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("MyBooks"));
            SelectPdfCommand = new RelayCommand(_ => SelectPdf());
        }

        public EditBookViewModel(MainViewModel mainViewModel, Book book) : this(mainViewModel)
        {
            Title = book.Title;
            Description = book.Description;
            PriceText = book.Price.ToString("F2");
            SelectedCategory = book.Category;
            PdfPath = book.PdfFilePath ?? string.Empty;
        }

        public string Title
        {
            get => _title;
            set { if (SetProperty(ref _title, value)) ValidateTitle(); }
        }

        public string Description
        {
            get => _description;
            set { if (SetProperty(ref _description, value)) ValidateDescription(); }
        }

        public string PriceText
        {
            get => _priceText;
            set { if (SetProperty(ref _priceText, value)) ValidatePrice(); }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set { if (SetProperty(ref _selectedCategory, value)) ValidateCategory(); }
        }

        public string PdfPath
        {
            get => _pdfPath;
            set { if (SetProperty(ref _pdfPath, value)) ValidatePdf(); }
        }

        public string TitleError
        {
            get => _titleError;
            set => SetProperty(ref _titleError, value);
        }

        public string CategoryError
        {
            get => _categoryError;
            set => SetProperty(ref _categoryError, value);
        }

        public string PriceError
        {
            get => _priceError;
            set => SetProperty(ref _priceError, value);
        }

        public string DescriptionError
        {
            get => _descriptionError;
            set => SetProperty(ref _descriptionError, value);
        }

        public string PdfError
        {
            get => _pdfError;
            set => SetProperty(ref _pdfError, value);
        }

        public List<string> Categories { get; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand SelectPdfCommand { get; }

        private bool ValidateTitle()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                TitleError = "Title is required.";
                return false;
            }
            if (Title.Trim().Length < 3)
            {
                TitleError = "Title must be at least 3 characters.";
                return false;
            }
            TitleError = string.Empty;
            return true;
        }

        private bool ValidateCategory()
        {
            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                CategoryError = "Please select a category.";
                return false;
            }
            CategoryError = string.Empty;
            return true;
        }

        private bool ValidatePrice()
        {
            if (string.IsNullOrWhiteSpace(PriceText))
            {
                PriceError = "Price is required.";
                return false;
            }
            if (!decimal.TryParse(PriceText, out decimal price))
            {
                PriceError = "Price must be a valid number.";
                return false;
            }
            if (price <= 0)
            {
                PriceError = "Price must be greater than 0.";
                return false;
            }
            if (price > 1000)
            {
                PriceError = "Price must not exceed $1000.";
                return false;
            }
            PriceError = string.Empty;
            return true;
        }

        private bool ValidateDescription()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                DescriptionError = "Description is required.";
                return false;
            }
            if (Description.Trim().Length < 10)
            {
                DescriptionError = "Description must be at least 10 characters.";
                return false;
            }
            DescriptionError = string.Empty;
            return true;
        }

        private bool ValidatePdf()
        {
            if (string.IsNullOrWhiteSpace(PdfPath))
            {
                PdfError = "Please upload the manuscript PDF.";
                return false;
            }
            PdfError = string.Empty;
            return true;
        }

        private bool ValidateAll()
        {
            bool titleOk = ValidateTitle();
            bool categoryOk = ValidateCategory();
            bool priceOk = ValidatePrice();
            bool descriptionOk = ValidateDescription();
            bool pdfOk = ValidatePdf();
            return titleOk && categoryOk && priceOk && descriptionOk && pdfOk;
        }

        private void SelectPdf()
        {
            var slug = string.IsNullOrWhiteSpace(Title) ? "manuscript" : Title.Trim().Replace(" ", "_").ToLowerInvariant();
            PdfPath = $"/Assets/Manuscripts/{slug}.pdf";
        }

        private void Save()
        {
            if (!ValidateAll())
            {
                return;
            }

            MessageBox.Show("Book updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _mainViewModel.NavigateTo("MyBooks");
        }
    }
}
