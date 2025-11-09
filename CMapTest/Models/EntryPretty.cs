using System.ComponentModel.DataAnnotations;

namespace CMapTest.Models
{
    public sealed class EntryPretty
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        [Display(Name = "User's Name")]
        public string UserPreferName { get; set; }
        [Display(Name = "Project Name")]
        public string ProjectName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        [Display(Name = "Work Period"), DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan WorkingPeriod { get; set; }
        public string? Description { get; set; }
    }
}
