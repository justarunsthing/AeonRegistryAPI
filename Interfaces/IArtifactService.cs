namespace AeonRegistryAPI.Interfaces
{
    public interface IArtifactService
    {
        // Public
        Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct);
        Task<List<PublicArtifactResponse>> GetPublicArtifactsBySiteAsync(int siteId, CancellationToken ct);
        Task<PublicArtifactResponse?> GetPublicArtifactByIdAsync(int artifactId, CancellationToken ct);

        // Private
        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsAsync(CancellationToken ct);
        Task<List<PrivateArtifactResponse>> GetPrivateArtifactsBySiteAsync(int siteId, CancellationToken ct);
        Task <PrivateArtifactResponse?> CreateArtifactAsync(CreateArtifactRequest request, CancellationToken ct);
    }
}
