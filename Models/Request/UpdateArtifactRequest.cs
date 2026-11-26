namespace AeonRegistryAPI.Models.Request
{
    public class UpdateArtifactRequest
    {
        [Required, MaxLength(200)]
        public string? Name { get; set; }

        [Required, MaxLength(500)]
        public string? CatalogNumber { get; set; }

        [Required, MaxLength(2000)]
        public string? InternalNarrative { get; set; }

        [Required, MaxLength(2000)]
        public string? PublicNarrative { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime Discovered { get; set; }
    }
}
