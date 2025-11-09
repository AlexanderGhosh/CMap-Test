using CMapTest.Data;
using CMapTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMapTest.Pages
{
    [Authorize(Policy = Policies.Admin)]
    public class ProjectsModel(IProjectsDataLayer _projects) : PageModel
    {
        public IEnumerable<Project> Projects { get; set; }
        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Projects = await _projects.GetAllProjects(default);
            return Page();
        }

        public async Task<IActionResult> OnPostAddProjectAsync(Project project)
        {
            await _projects.CreateProject(project, default);
            return Redirect(Request.Path);
        }

        public async Task<IActionResult> OnPostRemoveProject(int projId)
        {
            await _projects.RemoveProject(projId, default);
            return Page();
        }
    }
}
