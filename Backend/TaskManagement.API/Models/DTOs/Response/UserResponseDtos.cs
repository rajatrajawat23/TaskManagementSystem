namespace TaskManagement.API.Models.DTOs.Response
{
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int AssignedTasksCount { get; set; }
        public int CompletedTasksCount { get; set; }
        public List<string> Permissions { get; set; } = new();
        public string? CompanyName { get; set; }
    }

    public class UserPermissionDto
    {
        public string Permission { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class UserSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
    }
}