using Microsoft.EntityFrameworkCore;
using AeonRegistryAPI.Endpoints.Home;
using Microsoft.AspNetCore.Identity;

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

// Enable validation for minimal APIs
builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

var authRouteGroup = app.MapGroup("/api/auth").WithTags("Admin");
authRouteGroup.MapIdentityApi<ApplicationUser>();

app.MapHomeEndpoints();

app.Run();