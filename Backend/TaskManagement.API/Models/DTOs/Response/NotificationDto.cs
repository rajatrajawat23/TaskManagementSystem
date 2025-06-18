namespace TaskManagement.API.Models.DTOs.Response
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}