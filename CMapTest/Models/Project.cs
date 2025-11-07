using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    public class Project
    {
        [Required(AllowEmptyStrings = false)]
        public required int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public bool Enabled { get; set; }
        public string? Description { get; set; }
    }
}
