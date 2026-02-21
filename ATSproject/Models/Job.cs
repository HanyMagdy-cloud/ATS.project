using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class Job
    {
        public Guid JobId { get; set; }

        [Required]
        public Guid AccountId { get; set; }   // company (tenant)

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
