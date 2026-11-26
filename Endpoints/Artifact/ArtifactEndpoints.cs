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

            var privateGroup = route.MapGroup("/api/private/artifacts")
                .WithTags("Artifact - Private")
                .AddEndpointFilter<ExceptionHandlingFilter>()
                .RequireAuthorization();

            // Public endpoints
            publicGroup.MapGet("", GetPublicArtifacts)
                .WithName(nameof(GetPublicArtifacts))
                .WithSummary("Get all public artifacts")
                .WithDescription("Returns a list of all public artifacts.")
                .Produces<List<PublicArtifactResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            publicGroup.MapGet("/{id:int}", GetPublicArtifactById)
                .WithName(nameof(GetPublicArtifactById))
                .WithSummary("Get a public artifact by ID")
                .WithDescription("Returns a single artifact with public-safe data.")
                .Produces<PublicArtifactResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            // Private endpoints
            privateGroup.MapGet("", GetPrivateArtifacts)
                .WithName(nameof(GetPrivateArtifacts))
                .WithSummary("Get all private artifacts")
                .WithDescription("Returns a list of all private artifacts.")
                .Produces<List<PrivateArtifactResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapGet("/{id:int}", GetPrivateArtifactById)
                .WithName(nameof(GetPrivateArtifactById))
                .WithSummary("Get a private artifact by ID")
                .WithDescription("Returns a single artifact with private data.")
                .Produces<PrivateArtifactResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapPost("", CreateArtifact)
                .WithName(nameof(CreateArtifact))
                .WithSummary("Create a new artifact record")
                .WithDescription("Creates a new artifact with private data and returns the created artifact.")
                .Accepts<CreateArtifactRequest>("application/json")
                .ProducesValidationProblem()
                .Produces<PrivateArtifactResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError);

            privateGroup.MapPut("/{id:int}", UpdateArtifact)
                .WithName(nameof(UpdateArtifact))
                .WithSummary("Update an existing artifact record")
                .WithDescription("Updates an existing artifact with private data.")
                .Accepts<UpdateArtifactRequest>("application/json")
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
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

        private static async Task<Results<Ok<PublicArtifactResponse>, NotFound>> GetPublicArtifactById(int id, IArtifactService service, CancellationToken ct)
        {
            var artifact = await service.GetPublicArtifactByIdAsync(id, ct);

            if (artifact is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(artifact);
        }

        private static async Task<Results<Ok<List<PrivateArtifactResponse>>, NotFound>> GetPrivateArtifacts(IArtifactService service, CancellationToken ct)
        {
            var artifacts = await service.GetPrivateArtifactsAsync(ct);

            if (artifacts == null || artifacts.Count == 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(artifacts);
        }

        private static async Task<Results<Ok<PrivateArtifactResponse>, NotFound>> GetPrivateArtifactById(int id, IArtifactService service, CancellationToken ct)
        {
            var artifact = await service.GetPrivateArtifactByIdAsync(id, ct);

            if (artifact is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(artifact);
        }

        private static async Task<Results<Created<PrivateArtifactResponse>, NotFound>> CreateArtifact(IArtifactService service, CreateArtifactRequest request, CancellationToken ct)
        {
            var created = await service.CreateArtifactAsync(request, ct);

            if (created == null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Created($"/api/private/artifacts/{created.Id}", created);
        }

        private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateArtifact(int id, IArtifactService service, UpdateArtifactRequest request, CancellationToken ct)
        {
            var updated = await service.UpdateArtifactAsync(id, request, ct);

            if (!updated)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        }

    }
}
