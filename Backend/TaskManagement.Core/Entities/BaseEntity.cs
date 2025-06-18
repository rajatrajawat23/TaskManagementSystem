using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? CreatedById { get; set; }
        
        public Guid? UpdatedById { get; set; }
        
        public bool IsDeleted { get; set; } = false;
    }
}