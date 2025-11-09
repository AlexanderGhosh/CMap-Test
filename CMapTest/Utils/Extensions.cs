using System.Security.Claims;

namespace CMapTest.Utils
{
    public static class Extensions
    {
        extension(ClaimTypes)
        {
            public static string UserId => "CMapTest.ClaimTypes.UserId";
        }
        extension(ClaimsPrincipal user)
        {
            public int UserId
            {
                get
                {
                    string? cValue = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.UserId)?.Value;
                    if (cValue is null) throw new InvalidOperationException("Cannot find User Id claim");
                    return int.Parse(cValue);
                }
            }
        }
    }
}

