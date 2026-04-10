namespace AeonRegistryAPI.Interfaces
{
    public interface ISiteService
    {
        Task<PublicSiteResponse?> GetSiteByIdAsync(int id, CancellationToken ct);
        Task<List<PublicSiteResponse>> GetAllSitesAsync(CancellationToken ct);
        Task<PrivateSiteResponse?> GetPrivateSiteByIdAsync(int id, CancellationToken ct);
        Task<List<PrivateSiteResponse>> GetAllPrivateSitesAsync(CancellationToken ct);
        Task<PrivateSiteResponse> CreateSiteAsync(CreateSiteRequest request, CancellationToken ct);
        Task<bool> UpdateSiteAsync(int id, UpdateSiteRequest request, CancellationToken ct);
        Task<bool> DeleteSiteAsync(int id, CancellationToken ct);
    }
}