using Microsoft.EntityFrameworkCore;
using AeonRegistryAPI.Endpoints.Home;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.ConfigureCustomSwagger();

var connectionString = DataUtility.GetConnectionstring(builder.Configuration);
// Configure database context for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapHomeEndpoints();

app.Run();