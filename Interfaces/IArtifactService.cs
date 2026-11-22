namespace AeonRegistryAPI.Interfaces
{
    public interface IArtifactService
    {
        Task<List<PublicArtifactResponse>> GetPublicArtifactsAsync(CancellationToken ct);
    }
}
