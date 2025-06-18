using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Core.Entities
{
    [Table("TaskComments", Schema = "Core")]
    public class TaskComment : BaseEntity
    {
        [Required]
        public Guid TaskId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public bool IsInternal { get; set; } = false;

        // Navigation properties
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; } = null!;

        [ForeignKey("CreatedById")]
        public virtual User? User { get; set; }
    }
}
