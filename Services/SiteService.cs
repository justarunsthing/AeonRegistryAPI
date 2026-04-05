using AeonRegistryAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistryAPI.Services
{
    public class SiteService(ApplicationDbContext context) : ISiteService
    {
        public async Task<PublicSiteResponse?> GetSiteByIdAsync(int id, CancellationToken ct)
        {
            return await context.Sites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new PublicSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Coordinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    PublicNarrative = s.PublicNarrative
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct)
        {
            return await context.Sites
                .AsNoTracking()
                .Select(s => new PublicSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Coordinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    PublicNarrative = s.PublicNarrative
                })
                .ToListAsync(ct);
        }

        public async Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct)
        {
            return await context.Sites
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new PrivateSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Coordinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    InternalNarrative = s.InternalNarrative
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct)
        {
            return await context.Sites
                .AsNoTracking()
                .Select(s => new PrivateSiteResponse
                {
                    Id = s.Id,
                    Name = s.Name!,
                    Location = s.Location,
                    Coordinates = s.Coordinates,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Description = s.Description,
                    PublicNarrative = s.PublicNarrative,
                    InternalNarrative = s.InternalNarrative
                })
                .ToListAsync(ct);
        }
    }
}