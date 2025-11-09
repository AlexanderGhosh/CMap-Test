using CMapTest.Utils;
using System.ComponentModel.DataAnnotations;

namespace CMapTest.Config
{
    // I know about Microsoft.AspNetCore.Identity.PasswordOptions I felt that it didn't give enough config and i couldn't think of a better name (unless i expanded the code eg. MFA)
    public sealed class AuthOptions
    {
        public IntRange PasswordLength { get; init; } = new(12);
        public IntRange PasswordLowerCase { get; set; } = new(1);
        public IntRange PasswordUpperCase { get; set; } = new(1);
        public IntRange PasswordDigits { get; set; } = new(1);
        public bool AllowNonAlphaNumeric { get; set; } = true;
        public int SaltLength { get; set; } = 64;
        [Required(AllowEmptyStrings = false)]
        public required string AuthenticationIssuer { get; init; }
        [Required(AllowEmptyStrings = false)]
        public required string AuthenticationAudience { get; init; }
        [Required]
        public required TimeSpan CookieExpiry { get; init; }
    }
}
