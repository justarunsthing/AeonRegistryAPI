using AeonRegistryAPI.Filters;
using AeonRegistryAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AeonRegistryAPI.Endpoints.Artifact
{
    public static class ArtifactEndpoints
    {
        public static IEndpointRouteBuilder MapArtifactEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/public/artifacts")
                .WithTags("Artifact - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>()
                .AllowAnonymous();

            // Public endpoints
            publicGroup.MapGet("", GetPublicArtifacts)
                .WithName(nameof(GetPublicArtifacts))
                .WithSummary("Get all public artifacts")
                .WithDescription("Returns a list of all public artifacts.")
                .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            return route;
        }

        private static async Task<Results<Ok<List<PublicArtifactResponse>>, NotFound>> GetPublicArtifacts(IArtifactService service, CancellationToken ct)
        {
            var artifacts = await service.GetPublicArtifactsAsync(ct);

            if (artifacts == null || artifacts.Count == 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(artifacts);
        }
    }
}
