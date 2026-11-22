namespace AeonRegistryAPI.Models.Response
{
    public class PrivateArtifactResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CatalogNumber { get; set; }
        public string? InternalNarrative { get; set; } // Aeon narrative
        public string? PublicNarrative { get; set; } // Public narrative
        public DateTime Discovered { get; set; }
        public string? Type { get; set; }
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
