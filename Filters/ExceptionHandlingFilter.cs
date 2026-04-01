namespace AeonRegistryAPI.Filters
{
    public class ExceptionHandlingFilter : IEndpointFilter
    {
        // Tests if the method throws an exception
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            try
            {
                return await next(context);
            }
            catch (Exception ex)
            {
                var env = context.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();

                Console.WriteLine($"Exception caught in filter: {ex.Message}");
                
                return Results.Problem(
                    detail: env.IsDevelopment() ? ex.ToString() : null,
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An unexpected server error occurred."
                );
            }
        }
    }
}