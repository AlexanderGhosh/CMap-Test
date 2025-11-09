namespace CMapTest.Models
{
    public sealed class CreateEntry
    {
        public User User { get; set; }
        public Project Project { get; set; }

        public DateOnly Date { get; set; }

    }
}
