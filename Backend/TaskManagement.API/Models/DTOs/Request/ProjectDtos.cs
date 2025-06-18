using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models.DTOs.Request
{
    public class CreateProjectDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? Description { get; set; }

        public Guid? ClientId { get; set; }

        [Required]
        public Guid ManagerId { get; set; }

        [Required]
        public string Status { get; set; } = "Planning"; // Planning, Active, OnHold, Completed, Cancelled

        public decimal? Budget { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<string>? Tags { get; set; }

        public List<Guid>? TeamMemberIds { get; set; }
        
        public List<Guid>? TeamMembers { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid CreatedById { get; set; }
    }

    public class UpdateProjectDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(5000)]
        public string? Description { get; set; }

        public Guid? ClientId { get; set; }

        [Required]
        public Guid ManagerId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public decimal? Budget { get; set; }

        public decimal? ActualCost { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Progress { get; set; }

        public List<string>? Tags { get; set; }
        
        public List<Guid>? TeamMembers { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }

    public class AddTeamMemberDto
    {
        public Guid ProjectId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public string? Role { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
    }

    public class UpdateProjectStatusDto
    {
        public Guid ProjectId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Set by system
        public Guid CompanyId { get; set; }
        public Guid UpdatedById { get; set; }
    }
}