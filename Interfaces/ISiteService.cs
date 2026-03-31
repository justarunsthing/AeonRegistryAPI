namespace AeonRegistryAPI.Interfaces
{
    public interface ISiteService
    {
        Task<List<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct);
    }
}