using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class Account
    {
        // Primary key
        public Guid AccountId { get; set; }

        [Required]
        [StringLength(200)]
        public required  string Name { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
