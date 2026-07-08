using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using BookManagement.Helpers;

namespace BookManagement.ViewModels.Admin
{
    public class AdminPurchasesViewModel : BaseViewModel
    {
        private readonly DashboardViewModelBase _dashboard;
        private readonly IPurchaseService _purchaseService;
        private readonly IBookService _bookService;

        private string _searchText = string.Empty;
        private string _selectedReader = "All";
        private string _selectedStatus = "All";

        private double _totalRevenue;
        private int _totalTransactions;
        private int _completedCount;
        private int _cancelledCount;

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PurchasesView.Refresh();
                }
            }
        }

        public string SelectedReader
        {
            get => _selectedReader;
            set
            {
                if (SetProperty(ref _selectedReader, value))
                {
                    PurchasesView.Refresh();
                }
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (SetProperty(ref _selectedStatus, value))
                {
                    PurchasesView.Refresh();
                }
            }
        }

        public double TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public int TotalTransactions
        {
            get => _totalTransactions;
            set => SetProperty(ref _totalTransactions, value);
        }

        public int CompletedCount
        {
            get => _completedCount;
            set => SetProperty(ref _completedCount, value);
        }

        public int CancelledCount
        {
            get => _cancelledCount;
            set => SetProperty(ref _cancelledCount, value);
        }

        public ObservableCollection<PurchaseModel> Purchases { get; } = new ObservableCollection<PurchaseModel>();
        public ICollectionView PurchasesView { get; }

        public ObservableCollection<string> Readers { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Statuses { get; } = new ObservableCollection<string> { "All", "Completed", "Cancelled" };

        public ICommand ViewDetailCommand { get; }

        public AdminPurchasesViewModel(
            DashboardViewModelBase dashboard,
            IPurchaseService purchaseService,
            IBookService bookService)
        {
            _dashboard = dashboard;
            _purchaseService = purchaseService;
            _bookService = bookService;

            ViewDetailCommand = new RelayCommand<PurchaseModel>(OnViewDetail);

            PurchasesView = CollectionViewSource.GetDefaultView(Purchases);
            PurchasesView.Filter = FilterPurchases;

            LoadPurchases();
        }

        private void LoadPurchases()
        {
            Purchases.Clear();
            var list = _purchaseService.GetAllPurchases().ToList();
            foreach (var p in list)
            {
                Purchases.Add(p);
            }

            Readers.Clear();
            Readers.Add("All");
            var names = list.Select(p => p.ReaderName).Distinct().ToList();
            foreach (var name in names)
            {
                Readers.Add(name);
            }

            // Purchase statistics (based on completed transactions only for revenue)
            TotalTransactions = list.Count;
            CompletedCount = list.Count(p => p.Status == "Completed");
            CancelledCount = list.Count(p => p.Status == "Cancelled");
            TotalRevenue = list.Where(p => p.Status == "Completed").Sum(p => p.Price);
        }

        private bool FilterPurchases(object item)
        {
            if (!(item is PurchaseModel purchase)) return false;

            if (!string.IsNullOrEmpty(SearchText))
            {
                bool matchesSearch = purchase.BookTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                     purchase.ReaderName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                if (!matchesSearch) return false;
            }

            if (SelectedReader != "All")
            {
                if (purchase.ReaderName != SelectedReader) return false;
            }

            if (SelectedStatus != "All")
            {
                if (purchase.Status != SelectedStatus) return false;
            }

            return true;
        }

        private void OnViewDetail(PurchaseModel purchase)
        {
            if (purchase == null) return;
            _dashboard.ShowToast($"Chi tiết giao dịch #TXN{purchase.Id:D5}: {purchase.ReaderName} mua '{purchase.BookTitle}' giá ${purchase.Price:F2} ({purchase.Status})", "Info");
        }
    }
}
