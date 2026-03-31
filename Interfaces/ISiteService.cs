namespace AeonRegistryAPI.Interfaces
{
    public interface ISiteService
    {
        Task<IEnumerable<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct);
    }
}