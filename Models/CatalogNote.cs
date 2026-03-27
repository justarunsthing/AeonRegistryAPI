namespace AeonRegistryAPI.Models
{
    public class CatalogNote
    {
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        // Navigational Properties
        [Required]
        public int CatalogRecordId { get; set; }
        public CatalogRecord? CatalogRecord { get; set; } = null!;
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser? Author { get; set; } = null!;
    }
}