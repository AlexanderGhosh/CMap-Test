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
    public class IndexModel(IAuthDataLayer _authData) : PageModel
    {
        [BindProperty]
        public SignupUser Signup { get; set; } = new();
        public async Task<IActionResult> OnPostLogin(LoginRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                User user = await _authData.LoginUser(request, cancellationToken);
                IEnumerable<Claim> claims = await _authData.GetUserClaims(user.Id, cancellationToken);
                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal claimsPrincipal = new(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                return Redirect("/User");
            }
            catch (Exception)
            {
                return Redirect(Request.Path);
            }
        }

        public async Task<IActionResult> OnPostSignup(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid) return Redirect(Request.Path);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _authData.SignUpUser(Signup, cancellationToken);
                return Redirect("/User");
            }
            catch (OperationFailedException)
            {
                return Redirect(Request.Path);
            }
        }
    }
}
