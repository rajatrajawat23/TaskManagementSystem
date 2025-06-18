using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        [MaxLength(50)]
        public string NotificationType { get; set; }

        public Guid? RelatedEntityId { get; set; }

        [MaxLength(50)]
        public string RelatedEntityType { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        [MaxLength(10)]
        public string Priority { get; set; } = "Normal";

        public DateTime? ExpiryDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
    }
}