using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using AeonRegistryAPI.Endpoints.CustomIdentityEndpoints.Models;

namespace AeonRegistryAPI.Endpoints.CustomIdentityEndpoints
{
    public static class CustomIdentityEndpoints
    {
        public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
        {
            // Make a group
            var identityGroup = route.MapGroup("/api/auth")
                .WithTags("Admin");

            // Make endpoints in the group
            identityGroup.MapPost("/register-admin", RegisterUser)
                .WithName("RegisterAdmin")
                .WithSummary("Register a user")
                .WithDescription("User must have an admin role");
            //.RequireAuthorization("AdminPolicy");

            return route;
        }

        // Route handlers
        private static async Task<IResult> RegisterUser(RegisterUserRequest dto, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IConfiguration config)
        {
            if (await userManager.FindByEmailAsync(dto.Email) is not null)
            {
                return Results.BadRequest(new { Error = $"User with email {dto.Email} already exists" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var tempPassword = "TempPassword123!";
            var userCreated = await userManager.CreateAsync(user, tempPassword);

            if (!userCreated.Succeeded)
            {
                return Results.BadRequest(new { Error = userCreated.Errors });
            }

            if (await roleManager.RoleExistsAsync("Researcher"))
            {
                await userManager.AddToRoleAsync(user, "Researcher");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = config["BaseUrl"] ?? "https://localhost:7103";

            await emailSender.SendEmailAsync(
                dto.Email,
                "Welcome to Aeon Registry",
                $"""
                    Your account has been created. Please change your password by visiting: {baseUrl}/Setpassword.html
                    
                    {baseUrl}/Setpassword.html?email={dto.Email}&resetCode={encodedToken}
                """);
                
            return Results.Ok(new { Message = $"User {user.Email} created, password reset link sent" });
        }
    }
}