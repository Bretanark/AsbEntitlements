using Entitlements.Api.Infrastructure.Neo4j;
using Entitlements.Api.Services;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<Neo4jSettings>(builder.Configuration.GetSection("Neo4j"));
builder.Services.AddSingleton<IDriver>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<Neo4jSettings>>().Value;

    return GraphDatabase.Driver(
        settings.Uri,
        AuthTokens.Basic(settings.Username, settings.Password));
});
builder.Services.AddScoped<IEntitlementRepository, EntitlementRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
