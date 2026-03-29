using AeonRegistryAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AeonRegistryAPI.Endpoints.Home;
using Microsoft.AspNetCore.Identity.UI.Services;
using AeonRegistryAPI.Endpoints.CustomIdentityEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.ConfigureCustomSwagger();

// Configure database context for PostgreSQL
var connectionString = DataUtility.GetConnectionstring(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

// Add identity endpoints
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Admin policy
builder.Services.AddAuthorizationBuilder().AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

// Email service
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();

// Enable validation for minimal APIs
builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlockIdentityEndpoints>();

var authRouteGroup = app.MapGroup("/api/auth").WithTags("Admin");
authRouteGroup.MapIdentityApi<ApplicationUser>();

app.MapCustomIdentityEndpoints();
app.MapHomeEndpoints();

app.Run();