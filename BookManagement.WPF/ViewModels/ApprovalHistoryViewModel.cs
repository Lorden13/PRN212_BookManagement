using System;
using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class ApprovalHistoryViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private bool _hasRecords;

        public ApprovalHistoryViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            Records = new ObservableCollection<ApprovalRecord>();
            LoadRecords();
        }

        public ObservableCollection<ApprovalRecord> Records { get; }

        public bool HasRecords
        {
            get => _hasRecords;
            set => SetProperty(ref _hasRecords, value);
        }

        private void LoadRecords()
        {
            Records.Clear();
            var authorName = _mainViewModel.CurrentUser?.FullName ?? string.Empty;
            var records = MockDataService.ApprovalRecords
                .Where(r => r.AuthorName.Equals(authorName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var record in records)
                Records.Add(record);

            HasRecords = Records.Count > 0;
        }
    }
}
