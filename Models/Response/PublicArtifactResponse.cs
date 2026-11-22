namespace AeonRegistryAPI.Models.Response
{
    public class PublicArtifactResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CatalogNumber { get; set; }
        public string? PublicNarrative { get; set; } // Public narrative
        public DateTime Discovered { get; set; }
        public string? Type { get; set; } 
        public int SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? PrimaryImageUrl { get; set; }
    }
}
