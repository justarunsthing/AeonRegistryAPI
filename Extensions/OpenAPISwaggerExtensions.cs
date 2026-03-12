using Microsoft.OpenApi;

namespace AeonRegistryAPI.Extensions
{
    public static class OpenAPISwaggerExtensions
    {
        public static IServiceCollection ConfigureCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Aeon Registry API",
                    Version = "v1",
                    Description = """

                    <img src="/images/AeonRegistryLogoBLK.png" height="120" />

                    ## Aeon Research Division

                    Internal API for managing recovered artifacts and research data.
                    Provides secure access for field researchers and analysts.

                    ### Key Features:
                    - Site and artifact catalog
                    - Research record submissions
                    - Secure media storage
                    - User role management

                    """,
                    Contact = new OpenApiContact
                    {
                        Name = "Aeon Registry Team",
                        Url = new Uri("https://github.com/justarunsthing/AeonRegistryAPI"),
                        Email = "justarunsthing@outlook.com"
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and your valid JWT token."
                });
            });

            return services;
        }
    }
}