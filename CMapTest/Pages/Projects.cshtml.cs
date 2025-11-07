using CMapTest.Data;
using CMapTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMapTest.Pages
{
    public class ProjectsModel(IDataLayer _dataLayer) : PageModel
    {
        public async Task<IEnumerable<Project>> GetAllProjects() => await _dataLayer.GetAllProjects(default);

        public void OnGet()
        {
        }
        public void OnPost()
        {
            _ = 0;
        }

        public async Task<IActionResult> OnPostAddProjectAsync(Project project)
        {
            await _dataLayer.CreateProject(project, default);
            return Redirect(Request.Path);
        }
    }
}
