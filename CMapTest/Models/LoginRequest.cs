using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    public sealed class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        public required string Username { get; init; }
        [Required(AllowEmptyStrings = false)]
        public required string Password { get; init; }
    }
}
