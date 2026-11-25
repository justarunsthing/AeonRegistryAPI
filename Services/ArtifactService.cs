using AeonRegistryAPI.Enums;
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

        public async Task<List<PublicArtifactResponse>> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites
                .AsNoTracking()
                .AnyAsync(s => s.Id == siteId, ct);

            if (!siteExists)
            {
                return [];
            }

            return await db.Artifacts
                .AsNoTracking()
                .Include(a => a.Site)
                .Include(a => a.MediaFiles)
                .Where(a => a.SiteId == siteId)
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

        public async Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int artifactId, CancellationToken ct)
        {
            var artifact = await db.Artifacts
                .AsNoTracking()
                .Where(a => a.Id == artifactId)
                .Select(a => new PublicArtifactResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    CatalogNumber = a.CatalogNumber,
                    PublicNarrative = a.PublicNarrative,
                    Discovered = a.Discovered,
                    Type = a.Type,
                    SiteName = a.Site!.Name,
                    PrimaryImageUrl = a.MediaFiles
                        .Where(m => m.IsPrimary)
                        .Select(m => $"/api/public/artifacts/images/{m.Id}")
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);

            return artifact;
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

        public async Task<List<PrivateArtifactResponse>> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct)
        {
            var siteExists = await db.Sites
                .AsNoTracking()
                .AnyAsync(s => s.Id == siteId, ct);

            if (!siteExists)
            {
                return [];
            }

            return await db.Artifacts
                .AsNoTracking()
                .Include(a => a.Site)
                .Include(a => a.MediaFiles)
                .Where(a => a.SiteId == siteId)
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

        public async Task<PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct)
        {
            var site = await db.Sites
                .FindAsync(request.SiteId, ct);

            if (site == null)
            {
                return null;
            }

            if (!Enum.TryParse<ArtifactType>(request.Type, true, out var artifactType))
            {
                throw new ArgumentException($"Invalid artifact type: {request.Type}");
            }

            var artifact = new Artifact
            {
                Name = request.Name,
                CatalogNumber = request.CatalogNumber,
                InternalNarrative = request.InternalNarrative,
                PublicNarrative = request.PublicNarrative,
                Type = request.Type.ToString(),
                SiteId = request.SiteId,
                Discovered = request.Discovered
            };

            db.Artifacts.Add(artifact);
            await db.SaveChangesAsync(ct);

            return new PrivateArtifactResponse
            {
                Id = artifact.Id,
                Name = artifact.Name,
                CatalogNumber = artifact.CatalogNumber,
                InternalNarrative = artifact.InternalNarrative,
                PublicNarrative = artifact.PublicNarrative,
                Discovered = artifact.Discovered,
                Type = artifact.Type.ToString(),
                SiteId = artifact.SiteId,
                SiteName = site.Name,
                PrimaryImageUrl = null
            };
        }
    }
}
