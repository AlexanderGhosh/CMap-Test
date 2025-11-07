using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    // Why not a record: I dont like how they look and dont think their features is worth the developer experience/ readability
    // also what happens if one day your class needs functions you would likely want a class in that case any ways,
    // dev may not have the confidence/knowledge to safely change from a record to a class
    public sealed class Entry
    {
        [Required]
        public required int Id { get; set; }
        [Required]
        public required int UserId { get; init; }
        [Required]
        public required int ProjectId { get; init; }
        // I would use datetime so we store when they started without having to change this model
        [Required]
        public required DateOnly Date { get; init; }
        // I would use Timespan
        [Required]
        public required float HoursWorked { get; init; }
        public string? Description { get; init; }
    }
}
