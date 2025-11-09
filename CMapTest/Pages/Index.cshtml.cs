using CMapTest.Data;
using CMapTest.Exceptions;
using CMapTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CMapTest.Pages
{
    public class IndexModel(IDataLayer _dataLayer) : PageModel
    {

        public void OnGet()
        {
            _ = 0;
        }
        public async Task<IActionResult> OnPostLogin(LoginRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                User user = await _dataLayer.LoginUser(request, cancellationToken);
                IEnumerable<Claim> claims = await _dataLayer.GetUserClaims(user.Id, cancellationToken);
                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal claimsPrincipal = new(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                return Redirect("/Entries");
            }
            catch (Exception)
            {
                return Redirect(Request.Path);
            }
        }

        public async Task<IActionResult> OnPostSignup(SignupUser signup, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _dataLayer.SignUpUser(signup, cancellationToken);
                return Redirect("/Entries");
            }
            catch (OperationFailedException)
            {
                return Redirect(Request.Path);
            }

        }
    }
}
