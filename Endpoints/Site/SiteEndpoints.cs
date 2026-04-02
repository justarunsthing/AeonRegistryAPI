using AeonRegistryAPI.Filters;
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
                .WithTags("Sites - Public")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            publicGroup.MapGet("/{id:int}", GetSiteById)
                .WithName(nameof(GetSiteById))
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Get site by ID")
                .WithDescription("Retrieves a public site record by its id");

            publicGroup.MapGet("", GetAllSites)
                .WithName(nameof(GetAllSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all sites")
                .WithDescription("Retrieves all public site records");

            return route;
        }

        private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetSiteById(int id, ISiteService service, CancellationToken ct)
        {
            var site = await service.GetSiteByIdAsync(id, ct);

            if (site is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(site);
        }

        private static async Task<Ok<List<PublicSiteResponse>>> GetAllSites(ISiteService service, CancellationToken ct)
        {
            var sites = await service.GetAllSitesAsync(ct);

            return TypedResults.Ok(sites);
        }
    }
}