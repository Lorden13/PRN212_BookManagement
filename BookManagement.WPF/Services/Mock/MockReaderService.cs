using System.Collections.Generic;
using System.Linq;

namespace BookManagement.Services.Mock
{
    public class MockReaderService : IReaderService
    {
        private readonly List<ReaderModel> _readers = new List<ReaderModel>();

        public static List<int> FavoriteBookIds { get; } = new List<int> { 1, 3, 5 };

        public MockReaderService()
        {
            _readers.Add(new ReaderModel { Id = 1, Name = "John Smith", Email = "reader@gmail.com", JoinedDate = "2026-05-10", Phone = "0123 456 789", Address = "123 Main St, New York, NY", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 2, Name = "Emma Watson", Email = "emma.watson@gmail.com", JoinedDate = "2026-05-12", Phone = "0234 567 890", Address = "456 Oak Ave, Los Angeles, CA", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 3, Name = "Michael Brown", Email = "michael.b@gmail.com", JoinedDate = "2026-05-15", Phone = "0345 678 901", Address = "789 Pine Rd, Chicago, IL", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 4, Name = "Sophia Davis", Email = "sophia.davis@yahoo.com", JoinedDate = "2026-05-18", Phone = "0456 789 012", Address = "101 Elm St, Houston, TX", Status = "Inactive" });
            _readers.Add(new ReaderModel { Id = 5, Name = "William Wilson", Email = "william.w@gmail.com", JoinedDate = "2026-05-20", Phone = "0567 890 123", Address = "202 Maple Dr, Phoenix, AZ", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 6, Name = "Olivia Taylor", Email = "olivia.t@hotmail.com", JoinedDate = "2026-05-22", Phone = "0678 901 234", Address = "303 Cedar Ln, Philadelphia, PA", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 7, Name = "James Thomas", Email = "james.thomas@gmail.com", JoinedDate = "2026-05-25", Phone = "0789 012 345", Address = "404 Birch Way, San Antonio, TX", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 8, Name = "Isabella Jackson", Email = "isabella.j@gmail.com", JoinedDate = "2026-05-28", Phone = "0890 123 456", Address = "505 Redwood St, San Diego, CA", Status = "Inactive" });
            _readers.Add(new ReaderModel { Id = 9, Name = "Benjamin White", Email = "benjamin.w@yahoo.com", JoinedDate = "2026-06-01", Phone = "0901 234 567", Address = "606 Cypress Dr, Dallas, TX", Status = "Active" });
            _readers.Add(new ReaderModel { Id = 10, Name = "Mia Harris", Email = "mia.harris@gmail.com", JoinedDate = "2026-06-05", Phone = "0912 345 678", Address = "707 Willow Ave, San Jose, CA", Status = "Active" });
        }

        public IEnumerable<ReaderModel> GetAllReaders() => _readers;

        public ReaderModel GetReaderById(int id) => _readers.FirstOrDefault(r => r.Id == id);

        public void UpdateProfile(ReaderModel reader)
        {
            var existing = GetReaderById(reader.Id);
            if (existing != null)
            {
                existing.Name = reader.Name;
                existing.Email = reader.Email;
                existing.Phone = reader.Phone;
            }
        }
    }
}
