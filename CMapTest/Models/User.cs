using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    public sealed class User
    {
        [Required]
        public required int Id { get; set; }
        [Required, Display(Name = "First Name")]
        public required string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public required string LastName { get; set; }
        // could maybe be an array but i think that from a FE perspective its easier to just send 1 string and i cant grantee i would split it correctly in all cases
        [Display(Name = "Other Names")]
        public string? OtherNames { get; set; }
        // might produce wrong results but i think that in most cases it is fine
        public string FullName => string.Join(' ', FirstName, OtherNames, LastName);
        [Display(Name = "Preferred Name")]
        public string PreferredName
        {
            get => field ?? FirstName;
            set;
        }
    }
}
