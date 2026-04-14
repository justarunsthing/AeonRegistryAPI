using AeonRegistryAPI.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

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
    }
}