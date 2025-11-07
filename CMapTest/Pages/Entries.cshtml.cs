using CMapTest.Data;
using CMapTest.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMapTest.Pages
{
    public class EntriesModel(IDataLayer _dataLayer) : PageModel
    {
        // i enumerable so it is kinda lazy evaluated
        // i know there is IAsyncEnumerable but i dont want the individual projects to be awaits just the entire thing
        public async Task<IEnumerable<Project>> GetAllProjects() => await _dataLayer.GetAllProjects(default);

        public void OnGet()
        {
        }
    }
}
