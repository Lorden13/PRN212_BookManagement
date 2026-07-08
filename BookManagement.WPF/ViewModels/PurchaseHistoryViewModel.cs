using System.Collections.ObjectModel;
using BookManagement.WPF.Helpers;
using BookManagement.WPF.Models;

namespace BookManagement.WPF.ViewModels
{
    public class PurchaseHistoryViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        public PurchaseHistoryViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            LoadData();
        }

        public ObservableCollection<PurchaseRecord> Purchases { get; } = new();

        private bool _hasPurchases;
        public bool HasPurchases
        {
            get => _hasPurchases;
            set => SetProperty(ref _hasPurchases, value);
        }

        private void LoadData()
        {
            Purchases.Clear();
            foreach (var record in MockDataService.PurchaseRecords)
                Purchases.Add(record);

            HasPurchases = Purchases.Count > 0;
        }
    }
}
