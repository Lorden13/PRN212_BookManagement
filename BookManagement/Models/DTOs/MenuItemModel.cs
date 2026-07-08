using System.Windows.Input;

namespace BookManagement.Models.DTOs
{
    public class MenuItemModel
    {
        public string Title { get; set; } = string.Empty;
        public string? IconGeometryKey { get; set; }
        public string? ViewKey { get; set; }
        public ICommand? Command { get; set; }
    }
}
