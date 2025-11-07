using CMapTest.Data;
using CMapTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CMapTest.Pages
{
    public class IndexModel(IDataLayer _dataLayer) : PageModel
    {
        public async Task<IActionResult> OnPostLogin(LoginRequest request, CancellationToken cancellationToken)
        {
            await _dataLayer.LoginUser(request, default);
            return Redirect(Request.Path);
        }
    }
}
