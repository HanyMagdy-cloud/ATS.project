using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class CreateAdminUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string TempPassword { get; set; } = string.Empty;
    }
}
