using CMapTest.Models;

namespace CMapTest.Data
{
    public interface IEntriesDataLayer
    {
        Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken);
        Task<IEnumerable<EntryPretty>> GetAllEntries(CancellationToken cancellationToken);
        Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken);
        Task RemoveEntry(int id, CancellationToken cancellationToken);
        Task<EntryPretty> GetPretty(int id, CancellationToken cancellationToken);

        Task<IEnumerable<Entry>> EntrySearch(EntrySearchContext? search, CancellationToken cancellationToken);
    }
    public sealed class EntrySearchContext
    {
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
