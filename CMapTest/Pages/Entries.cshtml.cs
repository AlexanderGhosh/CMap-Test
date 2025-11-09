using CMapTest.Data;
using CMapTest.Models;
using CMapTest.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMapTest.Pages
{
    [Authorize(Policy = Policies.Admin)]
    public class EntriesModel(IEntriesDataLayer _entries, IProjectsDataLayer _projects, IUserDataLayer _users) : PageModel
    {
        [BindProperty]
        public Entry NewEntry { get; set; } = new Entry();
        public SelectList ProjectNames { get; set; }
        public SelectList UserPrefferedNames { get; set; }
        public IEnumerable<EntryPretty> Entries { get; set; }
        public EntrySearchContext SearchContext { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            ProjectNames = new SelectList(await _projects.GetAllProjects(default), nameof(Project.Id), nameof(Project.Name));
            UserPrefferedNames = new SelectList(await _users.GetAllUsers(default), nameof(Models.User.Id), nameof(Models.User.PreferredName));
            Entries = await _entries.GetAllEntries(default);
            Entries = applySearch(Entries, SearchContext);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewEntry.EndTime <= NewEntry.StartTime)
                ModelState.AddModelError($"{nameof(NewEntry)}.{nameof(Entry.EndTime)}", "End time must be in the future of start time");
            if (!ModelState.IsValid) return await OnGetAsync();
            NewEntry.UserId = User.UserId;
            await _entries.CreateEntry(NewEntry, default);
            return Redirect(Request.Path);
        }

        public async Task<IActionResult> OnPostRemoveEntry(int? entryId)
        {
            if (entryId is not null)
                await _entries.RemoveEntry(entryId.Value, default);
            return await OnGetAsync();
        }

        private IEnumerable<EntryPretty> applySearch(IEnumerable<EntryPretty> entries, EntrySearchContext? search)
        {
            return entries;
        }
    }
}
