namespace AeonRegistryAPI.Interfaces
{
    public interface ISiteService
    {
        Task<PublicSiteResponse?> GetSiteByIdAsync(int id, CancellationToken ct);
        Task<List<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct);
    }
}