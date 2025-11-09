using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMapTest.Pages
{
    public class ProjectModel(IProjectsDataLayer _projects, IEntriesDataLayer _entries) : PageModel
    {
        public Project Project { get; set; }
        public IEnumerable<EntryPretty> Entries { get; set; }
        public SelectList SelectableUsers { get; set; }
        public async Task<IActionResult> OnGetAsync(int? projectId, EntrySearchContext? search, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            search ??= new();
            search.ProjectId = projectId;
            if (projectId is null) return NotFound();
            try
            {
                Project = await _projects.GetProjectFromId(projectId.Value, cancellationToken);
                var entries = await _entries.EntrySearch(new EntrySearchContext() { ProjectId = projectId }, cancellationToken);

                // search for all posible so that users not included in the search still show up
                Entries = entries.Select(e => _entries.GetPretty(e.Id, cancellationToken).Result);
                SelectableUsers = new SelectList(Entries.DistinctBy(e => e.UserId), nameof(EntryPretty.UserId), nameof(EntryPretty.UserPreferName));

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

        public async Task<IActionResult> OnPostSearch(EntrySearchContext search, int? projectId)
        {
            return await OnGetAsync(projectId, search, default);
        }

        public async Task<IActionResult> OnPostClear(int? projectId)
        {
            return await OnGetAsync(projectId, null, default);
        }
    }
}
