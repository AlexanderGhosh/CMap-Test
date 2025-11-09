using CMapTest.Models;

namespace CMapTest.Data
{
    public interface IEntriesDataLayer
    {
        Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EntryPretty>> GetAllEntries(CancellationToken cancellationToken);
        Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken);
        Task RemoveEntry(int id, CancellationToken cancellationToken);
    }
}
