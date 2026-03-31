using AeonRegistryAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistryAPI.Services
{
    public class SiteService(ApplicationDbContext context) : ISiteService
    {
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
    }
}