using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class CreateApplication
    {
        [Required]
        public Guid JobId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? LinkedInUrl { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        public string? Notes { get; set; }
    }
}
