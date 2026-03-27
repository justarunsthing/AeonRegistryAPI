using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AeonRegistryAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<ArtifactMediaFile> ArtifactMediaFiles { get; set; }
        public DbSet<CatalogRecord> CatalogRecords { get; set; }
        public DbSet<CatalogNote> CatalogNotes { get; set; }
    }
}