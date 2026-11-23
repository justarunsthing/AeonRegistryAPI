using AeonRegistryAPI.Enums;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AeonRegistryAPI.Filters
{
    public class EnumStringFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(string) && context.MemberInfo?.Name == "Type")
            {
                schema.Description = "Allowed values: " + string.Join(", ", Enum.GetNames<ArtifactType>());
            }
        }
    }
}
