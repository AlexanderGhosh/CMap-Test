using CMapTest.Models;

namespace CMapTest.Data
{
    public interface IDataLayer
    {
        Task<User> GetUserFromId(int id, CancellationToken cancellationToken);
        Task<Project> GetProjectFromId(int id, CancellationToken cancellationToken);
        Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken);
        Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken);
    }
}
