using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    // Why not a record: I dont like how they look and dont think their features is worth the developer experience/ readability
    // also what happens if one day your class needs functions you would likely want a class in that case any ways,
    // dev may not have the confidence/knowledge to safely change from a record to a class
    public sealed class Entry
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProjectId { get; init; }

        [Required]
        public DateTime Date { get; init; }
        [Required]
        public TimeOnly StartTime { get; init; }
        [Required]
        public TimeOnly EndTime { get; init; }
        public TimeSpan TimeWorked => EndTime - StartTime;
        public string? Description { get; init; }
    }
}
