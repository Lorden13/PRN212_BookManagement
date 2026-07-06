using System.Collections.ObjectModel;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ModerationHistoryViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public ModerationHistoryViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            LoadRecords();
        }

        public ObservableCollection<ApprovalRecord> Records { get; } = new();

        public bool HasRecords => Records.Count > 0;

        private void LoadRecords()
        {
            Records.Clear();
            foreach (var record in MockDataService.ApprovalRecords)
                Records.Add(record);
            OnPropertyChanged(nameof(HasRecords));
        }
    }
}
