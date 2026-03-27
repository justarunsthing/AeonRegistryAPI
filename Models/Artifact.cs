namespace AeonRegistryAPI.Models
{
    public class Artifact
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string? Name { get; set; }

        [Required, MaxLength(500)]
        public string? CatalogNumber { get; set; }

        [Required, MaxLength(2000)]
        public string? InternalNarrative { get; set; } // Aeon narrative

        [MaxLength(2000)]
        public string? PublicNarrative { get; set; }
        public DateTime Discovered { get; set; }
        public string? Type { get; set; }

        // Navigational properties
        [Required]
        public int SiteId { get; set; }
        public Site? Site { get; set; }
        public List<ArtifactMediaFile> MediaFiles { get; set; } = [];
        public List<CatalogRecord> CatalogRecords { get; set; } = [];
    }
}