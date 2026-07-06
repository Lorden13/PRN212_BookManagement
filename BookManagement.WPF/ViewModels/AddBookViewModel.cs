using System.Collections.Generic;
using System.Windows;
using BookManagement.WPF.Commands;

namespace BookManagement.WPF.ViewModels
{
    public class AddBookViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _priceText = string.Empty;
        private string _selectedCategory = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public AddBookViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Categories = new List<string>
            {
                "Technology", "Fiction", "Business", "Poetry",
                "History", "Lifestyle", "Self-Help", "Science Fiction"
            };
            SubmitCommand = new RelayCommand(_ => Submit());
            CancelCommand = new RelayCommand(_ => _mainViewModel.NavigateTo("MyBooks"));
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string PriceText
        {
            get => _priceText;
            set => SetProperty(ref _priceText, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public List<string> Categories { get; }

        public RelayCommand SubmitCommand { get; }
        public RelayCommand CancelCommand { get; }

        private void Submit()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Title is required.";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                ErrorMessage = "Please select a category.";
                HasError = true;
                return;
            }

            if (!decimal.TryParse(PriceText, out decimal price) || price <= 0)
            {
                ErrorMessage = "Price must be a number greater than 0.";
                HasError = true;
                return;
            }

            MessageBox.Show("Book submitted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            _mainViewModel.NavigateTo("MyBooks");
        }
    }
}
