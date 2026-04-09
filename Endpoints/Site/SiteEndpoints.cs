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

            var privateGroup = route.MapGroup("/api/private/sites")
                .RequireAuthorization()
                .WithSummary("Private Site Endpoints")
                .WithDescription("Endpoints for retrieving private site information, requires authentication")
                .WithTags("Sites - Private")
                .AddEndpointFilter<ExceptionHandlingFilter>();

            // Public endpoints
            publicGroup.MapGet("/{id:int}", GetSiteById)
                .WithName(nameof(GetSiteById))
                .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get site by ID")
                .WithDescription("Retrieves a public site record by its id");

            publicGroup.MapGet("", GetAllSites)
                .WithName(nameof(GetAllSites))
                .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all sites")
                .WithDescription("Retrieves all public site records");

            // Private endpoints
            privateGroup.MapGet("/{id:int}", GetPrivateSiteById)
                .WithName(nameof(GetPrivateSiteById))
                .Produces<PrivateSiteResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get private site by ID")
                .WithDescription("Retrieves a private site record by its id, requires authentication");

            privateGroup.MapGet("", GetAllPrivateSites)
                .WithName(nameof(GetAllPrivateSites))
                .Produces<List<PrivateSiteResponse>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Get all private sites")
                .WithDescription("Retrieves all private site records, requires authentication");

            privateGroup.MapPost("", CreateSite)
                .WithName(nameof(CreateSite))
                .Accepts<CreateSiteRequest>("application/json")
                .Produces<PrivateSiteResponse>(StatusCodes.Status201Created)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status500InternalServerError)
                .WithSummary("Create a new site")
                .WithDescription("Creates a new site record and returns it, requires authentication");

            privateGroup.MapPut("/{id:int}", UpdateSite)
                .WithName(nameof(UpdateSite))
                .Accepts<UpdateSiteRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError)
                .ProducesValidationProblem()
                .WithSummary("Update an existing site")
                .WithDescription("Updates an existing site record by its id, requires authentication");

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

        private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById(int id, ISiteService service, CancellationToken ct)
        {
            var site = await service.GetPrivateSiteByIdAsync(id, ct);

            if (site is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(site);
        }

        private static async Task<Ok<List<PrivateSiteResponse>>> GetAllPrivateSites(ISiteService service, CancellationToken ct)
        {
            var sites = await service.GetAllPrivateSitesAsync(ct);

            return TypedResults.Ok(sites);
        }

        private static async Task<Results<Created<PrivateSiteResponse>, ValidationProblem>> CreateSite(CreateSiteRequest request, ISiteService service, CancellationToken ct)
        {
            if (request is null)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]>
                {
                    { "Error", new[] { "Please fill in the required fields." } },
                });
            }

            var createdSite = await service.CreateSiteAsync(request, ct);

            return TypedResults.Created($"/api/private/sites/{createdSite.Id}", createdSite);
        }

        private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateSite(int id, UpdateSiteRequest request, ISiteService service, CancellationToken ct)
        {
            var success = await service.UpdateSiteAsync(id, request, ct);

            return success ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}