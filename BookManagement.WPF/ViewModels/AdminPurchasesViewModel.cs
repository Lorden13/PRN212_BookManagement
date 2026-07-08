using System.Collections.ObjectModel;
using System.Linq;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class AdminPurchasesViewModel : BaseViewModel
    {
        private bool _hasPurchases;
        private decimal _totalRevenue;
        private int _totalTransactions;
        private int _completedCount;
        private int _cancelledCount;

        public AdminPurchasesViewModel(MainViewModel mainViewModel)
        {
            Purchases = new ObservableCollection<PurchaseRecord>();
            LoadPurchases();
        }

        public ObservableCollection<PurchaseRecord> Purchases { get; }

        public bool HasPurchases
        {
            get => _hasPurchases;
            set => SetProperty(ref _hasPurchases, value);
        }

        public decimal TotalRevenue
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

        private void LoadPurchases()
        {
            Purchases.Clear();
            var records = MockDataService.PurchaseRecords.ToList();

            foreach (var record in records)
                Purchases.Add(record);

            TotalRevenue = records.Where(r => r.Status == "Completed").Sum(r => r.Price);
            TotalTransactions = records.Count;
            CompletedCount = records.Count(r => r.Status == "Completed");
            CancelledCount = records.Count(r => r.Status == "Cancelled");
            HasPurchases = Purchases.Count > 0;
        }
    }
}
