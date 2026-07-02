using System;
using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Mock
{
    public class MockPurchaseService : IPurchaseService
    {
        private readonly List<PurchaseModel> _purchases = new List<PurchaseModel>();

        public MockPurchaseService()
        {
            // Seed 30 purchase records
            string[] dates = { "2026-05-20", "2026-05-18", "2026-05-15", "2026-05-12", "2026-05-10", "2026-05-08", "2026-05-05", "2026-05-01", "2026-04-28", "2026-04-25" };
            string[] readerNames = {
                "John Smith", "Emma Watson", "Michael Brown", "Sophia Davis", "William Wilson",
                "Olivia Taylor", "James Thomas", "Isabella Jackson", "Benjamin White", "Mia Harris"
            };
            
            // Generate mock entries
            for (int i = 1; i <= 30; i++)
            {
                int bookId = (i % 15) + 1; // approved books (1 to 15)
                string title = GetBookTitle(bookId);
                double price = GetBookPrice(bookId);
                int readerId = ((i - 1) % 10) + 1;
                string readerName = readerNames[readerId - 1];
                
                _purchases.Add(new PurchaseModel
                {
                    Id = i,
                    BookId = bookId,
                    BookTitle = title,
                    ReaderId = readerId,
                    ReaderName = readerName,
                    Price = price,
                    PurchaseDate = dates[i % dates.Length],
                    Status = i % 10 == 0 ? "Cancelled" : "Completed"
                });
            }
        }

        private string GetBookTitle(int id)
        {
            string[] titles = {
                "Clean Code", "The Pragmatic Programmer", "Design Patterns", "Refactoring", 
                "The Hobbit", "Harry Potter and the Sorcerer's Stone", "Harry Potter and the Chamber of Secrets", 
                "Zero to One", "The Lean Startup", "A Brief History of Time", "Cosmos", 
                "Sapiens: A Brief History of Humankind", "Guns, Germs, and Steel", "The Elements of Style", "Democracy in America"
            };
            return titles[(id - 1) % titles.Length];
        }

        private double GetBookPrice(int id)
        {
            double[] prices = { 9.99, 12.99, 13.99, 11.49, 7.99, 8.99, 9.99, 10.99, 11.99, 9.49, 10.50, 12.00, 10.99, 5.99, 8.50 };
            return prices[(id - 1) % prices.Length];
        }

        public IEnumerable<PurchaseModel> GetPurchaseHistory(int readerId) => _purchases.Where(p => p.ReaderId == readerId);

        public IEnumerable<PurchaseModel> GetAllPurchases() => _purchases;

        private string GetReaderName(int id)
        {
            string[] names = {
                "John Smith", "Emma Watson", "Michael Brown", "Sophia Davis", "William Wilson",
                "Olivia Taylor", "James Thomas", "Isabella Jackson", "Benjamin White", "Mia Harris"
            };
            if (id >= 1 && id <= 10) return names[id - 1];
            return "Demo Reader";
        }

        public void PurchaseBook(int readerId, int bookId)
        {
            _purchases.Add(new PurchaseModel
            {
                Id = _purchases.Count + 1,
                BookId = bookId,
                BookTitle = GetBookTitle(bookId),
                ReaderId = readerId,
                ReaderName = GetReaderName(readerId),
                Price = GetBookPrice(bookId),
                PurchaseDate = DateTime.Now.ToString("yyyy-MM-dd"),
                Status = "Completed"
            });
        }
    }
}
