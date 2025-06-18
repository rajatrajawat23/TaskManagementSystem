using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Core.Entities
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Domain { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactEmail { get; set; }

        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? SubscriptionType { get; set; } = "Free";

        public DateTime? SubscriptionExpiryDate { get; set; }

        public int MaxUsers { get; set; } = 10;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}