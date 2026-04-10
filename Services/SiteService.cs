using AeonRegistryAPI.Interfaces;
using AeonRegistryAPI.Models.Request;
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

        public async Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct)
        {
            var site = new Site
            {
                Name = request.Name,
                Location = request.Location,
                Description = request.Description,
                Coordinates = request.Coordinates,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                PublicNarrative = request.PublicNarrative,
                InternalNarrative = request.InternalNarrative
            };

            context.Sites.Add(site);
            await context.SaveChangesAsync(ct);

            return new PrivateSiteResponse
            {
                Id = site.Id,
                Name = site.Name!,
                Location = site.Location,
                Coordinates = site.Coordinates,
                Latitude = site.Latitude,
                Longitude = site.Longitude,
                Description = site.Description,
                PublicNarrative = site.PublicNarrative,
                InternalNarrative = site.InternalNarrative
            };
        }

        public async Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct)
        {
            var site = await context.Sites.FindAsync([id], ct);

            if (site == null)
            {
                return false;
            }

            site.Name = request.Name;
            site.Location = request.Location;
            site.Description = request.Description;
            site.Coordinates = request.Coordinates;
            site.Latitude = request.Latitude;
            site.Longitude = request.Longitude;
            site.PublicNarrative = request.PublicNarrative;
            site.InternalNarrative = request.InternalNarrative;

            await context.SaveChangesAsync(ct);

            return true;
        }

        public async Task<bool> DeleteSiteAsync(int id, CancellationToken ct)
        {
            var site = await context.Sites.FindAsync([id], ct);

            if (site == null)
            {
                return false;
            }

            context.Sites.Remove(site);
            await context.SaveChangesAsync(ct);

            return true;
        }
    }
}