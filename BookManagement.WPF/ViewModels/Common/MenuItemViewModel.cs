using System.Windows.Input;

namespace BookManagement.ViewModels.Common
{
    public class MenuItemViewModel : BaseViewModel
    {
        private string _title = string.Empty;
        private string? _iconGeometryKey;
        private string? _viewKey;
        private ICommand? _command;
        private bool _isSelected;

        public MenuItemViewModel(MenuItemModel model)
        {
            Title = model.Title;
            IconGeometryKey = model.IconGeometryKey;
            ViewKey = model.ViewKey;
            Command = model.Command;
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string? IconGeometryKey
        {
            get => _iconGeometryKey;
            set => SetProperty(ref _iconGeometryKey, value);
        }

        public string? ViewKey
        {
            get => _viewKey;
            set => SetProperty(ref _viewKey, value);
        }

        public ICommand? Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
