using CMapTest.Utils;

namespace CMapTest.Config
{
    // I know about Microsoft.AspNetCore.Identity.PasswordOptions I felt that it didn't give enough config and i couldn't think of a better name (unless i expanded the code eg. MFA)
    public sealed class PasswordOptions
    {
        public NumberRange PasswordLength { get; init; } = new(12);
        public NumberRange PasswordLowerCase { get; set; } = new(1);
        public NumberRange PasswordUpperCase { get; set; } = new(1);
        public NumberRange PasswordDigits { get; set; } = new(1);
        public bool AllowNonAlphaNumberic { get; set; } = true;
        public int SaltLength { get; set; } = 64;
    }
}
