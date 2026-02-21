using System.ComponentModel.DataAnnotations;

namespace ATS_project.Models
{
    public class Application
    {
        public Guid ApplicationId { get; set; }

        [Required]
        public Guid AccountId { get; set; }  // tenant

        [Required]
        public Guid CandidateId { get; set; }

        [Required]
        public Guid JobId { get; set; }

        [Required]
        [StringLength(50)]
        public string Stage { get; set; } = "Applied";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (optional but helpful)
        public Candidate? Candidate { get; set; }
        public Job? Job { get; set; }
    }
}
