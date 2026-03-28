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

        // Customize EF Core's default behaviour - defining relationships, FKs & conversions
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Set up all necessary configs
            base.OnModelCreating(builder);

            // Custom mappings
            // DeleteBehavior.Restrict prevents deleting a user if they have associated submissions
            builder.Entity<CatalogRecord>()
                .HasOne(cr => cr.SubmittedBy)
                .WithMany(u => u.SubmittedCatalogRecords)
                .HasForeignKey(cr => cr.SubmittedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CatalogRecord>()
                .HasOne(cr => cr.VerifiedBy)
                .WithMany(u => u.VerifiedCatalogRecords)
                .HasForeignKey(cr => cr.VerifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Store the enum as its string name
            builder.Entity<Artifact>()
                .Property(a => a.Type)
                .HasConversion<string>();
        }
    }
}