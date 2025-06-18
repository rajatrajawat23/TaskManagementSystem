using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateTaskRequestDto
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid? AssignedToId { get; set; }

        public Guid? ClientId { get; set; }

        public Guid? ProjectId { get; set; }

        public string Priority { get; set; } = "Medium";

        public string Category { get; set; }

        public string[] Tags { get; set; }

        public decimal? EstimatedHours { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsRecurring { get; set; }

        public RecurrencePatternDto RecurrencePattern { get; set; }
    }

    public class RecurrencePatternDto
    {
        public string Frequency { get; set; } // Daily, Weekly, Monthly, Yearly
        public int Interval { get; set; } = 1;
        public string[] DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Occurrences { get; set; }
    }
}