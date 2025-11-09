using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    // this and user could prop inherit from base class but i don't think there is much of a use case
    public sealed class SignupUser
    {
        [Required]
        public required string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        // could maybe be an array but i think that from a FE perspective its easier to just send 1 string and i cant grantee i would split it correctly in all cases
        public string? OtherNames { get; set; }
        public string PreferredName
        {
            get => field ?? FirstName;
            set;
        }
    }
}
