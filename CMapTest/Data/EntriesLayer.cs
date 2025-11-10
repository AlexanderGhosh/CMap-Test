using CMapTest.Exceptions;
using CMapTest.Models;
using System.Collections.Concurrent;

namespace CMapTest.Data
{
    public sealed partial class DataLayer : IEntriesDataLayer
    {
        private readonly ConcurrentDictionary<int, Entry> _entries = [];
        public Task<Entry> GetEntryFromId(int id, CancellationToken cancellationToken)
        {
            if (_entries.TryGetValue(id, out Entry? entry)) return Task.FromResult(entry);
            throw new OperationFailedException("$Cannot find Entry with Id: {id}");
        }
        public Task<IEnumerable<EntryPretty>> GetAllEntries(CancellationToken cancellationToken)
        {
            var entriesRaw = _entries.Values;
            var res = entriesRaw.Join(_users, e => e.UserId, kvp => kvp.Key, (e, uKvp) => new EntryPretty()
            {
                Id = e.Id,
                Date = e.Date,
                UserPreferName = uKvp.Value.PreferredName,
                WorkingPeriod = e.TimeWorked,
                Description = e.Description
            });
            res = res.Join(_projects, e => e.ProjectId, kvp => kvp.Key, (e, pKvp) =>
            {
                e.ProjectName = pKvp.Value.Name;
                return e;
            });
#if DEBUG
            var tmp = res?.ToArray();
#endif
            return Task.FromResult(res ?? []);
        }
        public Task<Entry> CreateEntry(Entry entry, CancellationToken cancellationToken)
        {
            assertUserExists(entry.UserId);
            assertProjectExists(entry.ProjectId);
            if (isDuplicateEntry(entry)) throw new OperationFailedException("Cannot add duplicate entries");
            entry.Id = nextId(_entries); // assigned to variable so i can debug easier if needed
            _entries.TryAdd(entry.Id, entry);
            return Task.FromResult(entry);
        }
        public Task RemoveEntry(int id, CancellationToken cancellationToken)
        {
            assertEntryExists(id);
            _entries.TryRemove(id, out _);
            return Task.CompletedTask;
        }
        public async Task<EntryPretty> GetPretty(int id, CancellationToken cancellationToken)
        {
            assertEntryExists(id);
            cancellationToken.ThrowIfCancellationRequested();
            Entry e = await GetEntryFromId(id, cancellationToken);
            User u = await GetUserFromId(e.UserId, cancellationToken);
            Project p = await GetProjectFromId(e.ProjectId, cancellationToken);
            return new EntryPretty
            {
                Id = e.Id,
                Date = e.Date,
                UserPreferName = u.PreferredName,
                ProjectName = p.Name,
                Description = e.Description,
                UserId = e.UserId,
                ProjectId = e.ProjectId,
                WorkingPeriod = e.TimeWorked
            };
        }
        public Task<IEnumerable<Entry>> EntrySearch(EntrySearchContext? search, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<Entry> res = _entries.Values;
            if (search is null) return Task.FromResult(res);

            if (search.UserId is not null)
                res = res.Where(e => e.UserId == search.UserId);

            if (search.ProjectId is not null)
                res = res.Where(e => e.ProjectId == search.ProjectId);

            if (search.DateStart is not null)
                res = res.Where(e => e.Date >= search.DateStart);

            if (search.DateEnd is not null)
                res = res.Where(e => e.Date <= search.DateEnd);

            return Task.FromResult(res);
        }
        public Task UpdateEntry(Entry entry, CancellationToken cancellationToken)
        {
            assertEntryExists(entry.Id);
            _entries.AddOrUpdate(entry.Id, entry, (key, oldValue) => entry);
            return Task.CompletedTask;
        }


        // doesnt consider working on multiple projects at the same time or overlapping time periods
        private bool isDuplicateEntry(Entry candidate)
        {
            return _entries.Any(kvp =>
            {
                Entry e = kvp.Value;
                return e.UserId == candidate.UserId && e.ProjectId == candidate.ProjectId && e.Date == candidate.Date;
            });
        }
        private void assertEntryExists(int entryId)
        {
            if (!_entries.ContainsKey(entryId))
                throw new OperationFailedException($"Cannot find Entry with id {entryId}");
        }
    }
}
