namespace TaskManagement.API.Models.DTOs.Request
{
    public class TaskFilterRequestDto
    {
        public string SearchTerm { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ProjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeArchived { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "DESC";
    }
}