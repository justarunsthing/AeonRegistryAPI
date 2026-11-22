using AeonRegistryAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistryAPI.Services
{
    public class ArtifactService(ApplicationDbContext db) : IArtifactService
    {
        public async Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Include(a => a.Site)
                .Include(a => a.MediaFiles)
                .Select(a => new PublicArtifactResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    CatalogNumber = a.CatalogNumber,
                    PublicNarrative = a.PublicNarrative,
                    Discovered = a.Discovered,
                    Type = a.Type,
                    SiteId = a.SiteId,
                    SiteName = a.Site != null ? a.Site.Name : string.Empty,
                    PrimaryImageUrl = a.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => $"/api/public/artifacts/images/{m.Id}")
                        .FirstOrDefault()
                }).ToListAsync(ct);
        }

        public async Task<List<PrivateArtifactResponse>> GetPrivateArtifactsAsync(CancellationToken ct)
        {
            return await db.Artifacts
                .AsNoTracking()
                .Include(a => a.Site)
                .Include(a => a.MediaFiles)
                .Select(a => new PrivateArtifactResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    CatalogNumber = a.CatalogNumber,
                    InternalNarrative = a.InternalNarrative,
                    PublicNarrative = a.PublicNarrative,
                    Discovered = a.Discovered,
                    Type = a.Type,
                    SiteId = a.SiteId,
                    SiteName = a.Site != null ? a.Site.Name : string.Empty,
                    PrimaryImageUrl = a.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => $"/api/public/artifacts/images/{m.Id}")
                        .FirstOrDefault()
                }).ToListAsync(ct);
        }
    }
}
