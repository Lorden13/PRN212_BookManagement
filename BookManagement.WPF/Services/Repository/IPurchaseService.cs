using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IPurchaseService
    {
        IEnumerable<PurchaseModel> GetPurchaseHistory(int readerId);
        IEnumerable<PurchaseModel> GetAllPurchases();
        void PurchaseBook(int readerId, int bookId);
    }
}
