using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    public sealed class User
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        // could maybe be an array but i think that from a FE perspective its easier to just send 1 string and i cant grantee i would split it correctly in all cases
        public string? OtherNames { get; set; }
        // might produce wrong results but i think that in most cases it is fine
        public string FullName => string.Join(' ', FirstName, OtherNames, LastName);
        public string PreferredName
        {
            get => field ?? FirstName;
            set;
        }
    }
}
