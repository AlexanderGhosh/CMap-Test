namespace CMapTest.Models
{
    public sealed class EntryPretty
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string UserPreferName { get; set; }
        public string ProjectName { get; set; }
        public DateOnly Date { get; set; }
        public string WorkingPeriod { get; set; }
        public TimeSpan WorkingPeriodRaw { get; set; }
        public string? Description { get; set; }
    }
}
