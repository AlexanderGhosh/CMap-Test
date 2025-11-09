using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using CMapTest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMapTest.Pages
{
    [Authorize]
    public class UserModel(IUserDataLayer _users, IEntriesDataLayer _entries) : PageModel
    {
        public User AppUser { get; set; }
        public IEnumerable<EntryPretty> Entries { get; set; }
        public SelectList SelectableProjects { get; set; }
        [BindProperty]
        public Entry NewEntry { get; set; } = new();
        public async Task<IActionResult> OnGetAsync(EntrySearchContext? search, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            search ??= new();
            search.UserId = base.User.UserId;
            try
            {
                AppUser = await _users.GetUserFromId(search.UserId.Value, cancellationToken);
                var entries = await _entries.EntrySearch(new EntrySearchContext() { UserId = search.UserId }, cancellationToken);

                // search for all possible so that projects not included in the search still show up
                Entries = entries.Select(e => _entries.GetPretty(e.Id, cancellationToken).Result);
                SelectableProjects = new SelectList(Entries.DistinctBy(e => e.ProjectId), nameof(EntryPretty.Id), nameof(EntryPretty.ProjectName));

                // do the search again with the real filter
                entries = await _entries.EntrySearch(search, cancellationToken);
                Entries = entries.Select(e => _entries.GetPretty(e.Id, cancellationToken).Result);
            }
            catch (OperationFailedException)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(EntrySearchContext? search, CancellationToken cancellationToken) => await OnGetAsync(search, cancellationToken);

        public async Task<IActionResult> OnPostMakeEntryAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (NewEntry.EndTime <= NewEntry.StartTime)
                ModelState.AddModelError($"{nameof(NewEntry)}.{nameof(Entry.EndTime)}", "End time must be in the future of start time");
            if (!ModelState.IsValid) return await OnGetAsync(null, default);
            await _entries.CreateEntry(NewEntry, cancellationToken);
            return await OnGetAsync(null, cancellationToken);
        }
        public async Task<IActionResult> OnPostRemoveEntry(int? entryId)
        {
            if (entryId is not null)
                await _entries.RemoveEntry(entryId.Value, default);
            return await OnGetAsync(null, default);
        }

        public async Task<IActionResult> OnPostSearch(EntrySearchContext search)
        {
            return await OnGetAsync(search, default);
        }

        public async Task<IActionResult> OnPostClear()
        {
            return await OnGetAsync(null, default);
        }
    }
}
