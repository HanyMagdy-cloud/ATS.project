using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Null for Admin, required for Customer
        public Guid? AccountId { get; set; }

        [StringLength(200)]
        public string? FullName { get; set; }
    }
}
