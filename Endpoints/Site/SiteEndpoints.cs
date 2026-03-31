using AeonRegistryAPI.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AeonRegistryAPI.Endpoints.Site
{
    public static class SiteEndpoints
    {
        public static IEndpointRouteBuilder MapSiteEndpoints(this IEndpointRouteBuilder route)
        {
            var publicGroup = route.MapGroup("/api/sites")
                .AllowAnonymous()
                .WithSummary("Public Site Endpoints")
                .WithDescription("Endpoints for retrieving public site information")
                .WithTags("Sites - Public");

            publicGroup.MapGet("", GetAllSites)
                .WithName(nameof(GetAllSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all sites")
                .WithDescription("Retrieves all public site records");

            return route;
        }

        private static async Task<Ok<List<PublicSiteResponse>>> GetAllSites(ISiteService service, CancellationToken ct)
        {
            var sites = await service.GetAllSitesAsync(ct);

            return TypedResults.Ok(sites);
        }
    }
}