namespace AeonRegistryAPI.Interfaces
{
    public interface IArtifactService
    {
        Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct);
        Task<List<PublicArtifactResponse>> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct);
        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsAsync(CancellationToken ct);
        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct);
    }
}
