using AeonRegistryAPI.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using AeonRegistryAPI.Interfaces;

namespace AeonRegistryAPI.Endpoints.Artifact
{
    public static class ArtifactMediaFileEndpoints
    {
        public static IEndpointRouteBuilder MapArtifactMediaFileEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/public/artifacts/images")
                .WithTags("Artifact Media - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>()
                .AllowAnonymous();

            var privateGroup = route.MapGroup("/api/private/artifacts/{artifactId}/images")
                .WithTags("Artifact Media - Private")
                .RequireAuthorization()
                .AddEndpointFilter<ExceptionHandlingFilter>();

            // Public endpoints
            publicGroup.MapGet("/{id:int}", GetPublicArtifactImage)
                .WithName(nameof(GetPublicArtifactImage))
                .Produces<FileContentHttpResult>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Retrieves a public artifact image by its id")
                .WithDescription("""
                    Retrieves binary image data for a specific artiface media record.
                    This endpoint does not require authentication.
                    All unhandled exceptions are processed by the ExceptionHandlingFilter.
                    """);

            // Private endpoints
            privateGroup.MapPost("", CreateArtifactMediaFile)
                .DisableAntiforgery() // Disable CSRF protection for API endpoints
                .WithName(nameof(CreateArtifactMediaFile))
                .Accepts<IFormFile>("multipart/form-data")
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Upload Artiface Image (Private)")
                .WithDescription("""
                    Uploads an image file and associates it with an existing artifact.
                    Optional flag 'isPrimary' can be provided in the query or form data.
                    Requires authentication, all unhandled exceptions are processed by the ExceptionHandlingFilter.
                    """);

            return route;
        }

        private static async Task<Results<FileContentHttpResult, NotFound>> GetPublicArtifactImage(int id, ApplicationDbContext db, HttpResponse response, CancellationToken ct)
        {
            var image = await db.ArtifactMediaFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);

            if (image is null || image.Data.Length == 0)
            {
                return TypedResults.NotFound();
            }

            // Add client-side caching for performance
            response.Headers.CacheControl = "public, max-age=86400"; // Cache for 1 day

            return TypedResults.File(image.Data, image.ContentType);
        }

        private static async Task<Results<Created, NotFound, BadRequest>> CreateArtifactMediaFile(int artifactId, IFormFile file, bool isPrimary, IArtifactMediaFileService service, CancellationToken ct)
        {
            if (file is null || file.Length == 0)
            {
                return TypedResults.BadRequest();
            }

            var media = await service.CreateArtifactMediaFileAsync(artifactId, file, isPrimary, ct);

            if (media is null)
            {
                return TypedResults.NotFound();
            }

            var location = $"/api/public/artifacts/images/{media.Id}";

            return TypedResults.Created(location);
        }
    }
}