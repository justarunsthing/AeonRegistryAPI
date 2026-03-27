using AeonRegistryAPI.Enums;

namespace AeonRegistryAPI.Models
{
    public class CatalogRecord
    {
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = CatalogStatus.Draft.ToString();

        [Required]
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        // Navigational properties
        [Required]
        public int ArtifactId { get; set; }
        public Artifact Artifact { get; set; } = null!;

        [Required]
        public string SubmittedById { get; set; } = string.Empty;
        public ApplicationUser SubmittedBy { get; set; } = null!;
        public string? VerifiedById { get; set; }
        public ApplicationUser? VerifiedBy { get; set; } = null!;
        public ICollection<CatalogNote> Notes { get; set; } = [];
    }
}