namespace TaskManagement.API.Models.DTOs.Request
{
    public class UpdateTaskRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProjectId { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string[] Tags { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? Progress { get; set; }
        public bool? IsRecurring { get; set; }
        public RecurrencePatternDto RecurrencePattern { get; set; }
        public bool? IsArchived { get; set; }
    }
}