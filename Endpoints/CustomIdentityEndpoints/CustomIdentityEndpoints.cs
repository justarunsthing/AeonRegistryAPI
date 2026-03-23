using System.Text;
using System.Security.Claims;
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

            identityGroup.MapPost("/forgot-password", ForgotPassword)
                .WithName("ForgotPassword")
                .WithSummary("Custom forgot password")
                .WithDescription("Custom forgot password flow for a user")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);

            identityGroup.MapGet("/manage/profile", GetProfile)
                .WithName("ManageProfile")
                .WithSummary("Get the current user's profile")
                .WithDescription("Manage user's profile")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

            identityGroup.MapPut("/manage/profile", UpdateProfile)
                .WithName("UpdateProfile")
                .WithSummary("Update the current user's profile")
                .WithDescription("Update user's profile")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();

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

        private static async Task<IResult> ForgotPassword(ForgotPasswordRequest request, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return Results.BadRequest(new { Error = "Email address is required" });
            }

            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                // Return Ok to not let user know that user doesn't exists
                return Results.Ok(new { Message = "If the user exists, a forgot password link will be sent" });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var baseUrl = config["BaseUrl"] ?? "https://localhost:7103";
            var resetPasswordLink = $"{baseUrl}/reset-password.html?email={user.Email}&resetCode={encodedToken}";

            await emailSender.SendEmailAsync(
                request.Email,
                "Welcome to Aeon Registry",
                $"""
                    To reset your password, use this link:

                    {resetPasswordLink}
                """);

            return Results.Ok(new { Message = "If the user exists, a forgot password link will be sent" });
        }

        // Typically need ClaimsPrincipal for authenticated endpoints to get the logged in user
        private static async Task<IResult> GetProfile(ClaimsPrincipal principal, UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                return Results.NotFound();
            }

            var response = new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
            };

            return Results.Ok(response);
        }

        private static async Task<IResult> UpdateProfile(UpdateUserProfileRequest request,ClaimsPrincipal principal, UserManager<ApplicationUser> userManager)
        {
            if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
            {
                return Results.BadRequest(new { Error = "First and last names are required" });
            }

            var user = await userManager.GetUserAsync(principal);

            if (user is null)
            {
                return Results.NotFound();
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(new { Error = $"Update failed: {result.Errors}" });
            }

            return Results.Ok(new { Message = "User profile updated successfully" });
        }
    }
}