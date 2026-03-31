using AeonRegistryAPI.Interfaces;

namespace AeonRegistryAPI.Services
{
    public class SiteService(ApplicationDbContext context) : ISiteService
    {
        public Task<IEnumerable<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}