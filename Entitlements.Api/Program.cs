using Entitlements.Api.Contracts;
using Entitlements.Api.Infrastructure.Neo4j;
using Entitlements.Api.Seed;
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
builder.Services.AddScoped<IEntitlementCheckService, EntitlementCheckService>();
builder.Services.AddSingleton<DemoDataSeeder>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();

    var seeder = app.Services.GetRequiredService<DemoDataSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();

app.MapPost("/entitlements/check", async (
        EntitlementCheckRequest request,
        IEntitlementCheckService service,
        CancellationToken cancellationToken) =>
    {
        if (string.IsNullOrWhiteSpace(request.SubjectId))
            return Results.BadRequest("subjectId is required.");

        if (string.IsNullOrWhiteSpace(request.PermissionName))
            return Results.BadRequest("permissionName is required.");

        if (string.IsNullOrWhiteSpace(request.ResourceId))
            return Results.BadRequest("resourceId is required.");

        var response = await service.CheckAsync(request, cancellationToken);

        return Results.Ok(response);
    })
    .WithName("CheckEntitlement");

app.Run();
