using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.Data;
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
                .WithDescription("User must have an admin role")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);
            //.RequireAuthorization("AdminPolicy");

            identityGroup.MapPost("/reset-password", ResetPassword)
                .WithName("ResetPassword")
                .WithSummary("Custom reset password")
                .WithDescription("Custom reset password for a user")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            return route;
        }

        // Route handlers
        private static async Task<IResult> RegisterUser(RegisterUserRequest request, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IConfiguration config)
        {
            if (await userManager.FindByEmailAsync(request.Email) is not null)
            {
                return Results.BadRequest(new { Error = $"User with email {request.Email} already exists" });
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
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
                request.Email,
                "Welcome to Aeon Registry",
                $"""
                    Your account has been created. Please change your password by visiting: {baseUrl}/Setpassword.html
                    
                    {baseUrl}/Setpassword.html?email={request.Email}&resetCode={encodedToken}
                """);
                
            return Results.Ok(new { Message = $"User {user.Email} created, password reset link sent" });
        }

        private static async Task<IResult> ResetPassword(ResetPasswordRequest request, UserManager<ApplicationUser> userManager)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.ResetCode) || string.IsNullOrEmpty(request.NewPassword))
            {
                return Results.BadRequest(new { Error = "All fields are required" });
            }

            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Results.BadRequest(new { Error = "User not found" });
            }

            try
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.ResetCode));
                var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

                if (result.Succeeded)
                {
                    return Results.Ok(new { Message = "Password reset successful" });
                }

                return Results.BadRequest(new { Error = "Password reset unsuccessful" });
            }
            catch (FormatException)
            {
                return Results.BadRequest(new { Error = "Invalid token" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { Error = $"Error: {ex.Message}" });
            }
        }
    }
}