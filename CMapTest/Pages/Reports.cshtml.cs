using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CMapTest.Pages
{
    public class ReportsModel(IReportGenerator _reports, IUserDataLayer _users) : PageModel
    {
        public SelectList SelectableUsers { get; set; }
        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SelectableUsers = new SelectList(await _users.GetAllUsers(cancellationToken), nameof(Models.User.Id), nameof(Models.User.FullName));
            return Page();
        }

        public async Task<IActionResult> OnPostUserReportAsync(int? userId, DateTime? startDate, DateTime? endDate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId is null) ModelState.AddModelError(nameof(userId), "User id must not be null");
            if (ModelState.IsValid)
            {
                try
                {
                    DownloadableFile file = await _reports.GenerateUserReport(userId.Value, new(startDate, endDate), cancellationToken);
                    if (!file.IsValid) throw new InvalidProgramException("Downloadable file must have either a file path or byte[]");
                    if (file.Data is null)
                        return File(file.FilePath, file.ContentType, file.DownloadName);

                    return File(file.Data, file.ContentType, file.DownloadName);
                }
                catch (OperationFailedException)
                {
                    return NotFound();
                }
            }
            return await OnGetAsync(cancellationToken);
        }
    }
}
