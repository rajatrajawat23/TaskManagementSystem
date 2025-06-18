using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Core.Entities
{
    [Table("TaskAttachments", Schema = "Core")]
    public class TaskAttachment : BaseEntity
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [StringLength(100)]
        public string? FileType { get; set; }

        public Guid UploadedById { get; set; }

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; } = null!;

        [ForeignKey("UploadedById")]
        public virtual User UploadedBy { get; set; } = null!;
    }
}
