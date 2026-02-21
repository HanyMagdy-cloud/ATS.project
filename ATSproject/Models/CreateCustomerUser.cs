using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class CreateCustomerUser
    {
        [Required]
        public Guid AccountId { get; set; }

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
