using System.Collections.Generic;

namespace BookManagement.Services.Repository
{
    public interface IReaderService
    {
        IEnumerable<ReaderModel> GetAllReaders();
        ReaderModel GetReaderById(int id);
        void UpdateProfile(ReaderModel reader);
    }
}
