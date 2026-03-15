namespace AeonRegistryAPI.Middleware
{
    public class BlockIdentityEndpoints(RequestDelegate next)
    {
        private static readonly string[] BlockedPaths = 
        [
            "/api/auth/register",
            "/api/auth/resetpassword",
            "/api/auth/forgotpassword",
            "/api/auth/manage/info",
            "/api/auth/manage/profile"
        ];

        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (path is not null && BlockedPaths.Contains(path))
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;

                await context.Response.WriteAsync("Not Found!");

                return;
            };

            await _next(context);
        }
    }
}