namespace AeonRegistryAPI.Models
{
    public class ArtifactMediaFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = "image/jpeg";
        public byte[] Data { get; set; } = [];
        public bool IsPrimary { get; set; } = false;

        // Navigational properties
        public int ArtifactId { get; set; }
        public Artifact? Artifact { get; set; } = null!;
    }
}