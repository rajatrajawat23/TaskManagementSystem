using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public string Message { get; set; }

        [MaxLength(20)]
        public string MessageType { get; set; } = "Text";

        [MaxLength(500)]
        public string AttachmentUrl { get; set; }

        public bool IsEdited { get; set; } = false;

        public DateTime? EditedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public string ReadBy { get; set; } // JSON array of user IDs who have read

        // Navigation properties
        public virtual ChatGroup Group { get; set; }
        public virtual User Sender { get; set; }
    }
}