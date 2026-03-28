using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AeonRegistryAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        // Navigational Properties
        public ICollection<CatalogRecord> SubmittedCatalogRecords { get; set; } = [];
        public ICollection<CatalogRecord> VerifiedCatalogRecords { get; set; } = [];
        public ICollection<ArtifactMediaFile> UploadedMedia { get; set; } = [];
    }
}