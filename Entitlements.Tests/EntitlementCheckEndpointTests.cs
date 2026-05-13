using System.Net;
using System.Net.Http.Json;
using Entitlements.Api.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.Neo4j;

namespace Entitlements.Tests;

public sealed class EntitlementCheckEndpointTests(EntitlementApiFixture fixture)
    : IClassFixture<EntitlementApiFixture>
{

    [Fact]
    public async Task Check_ReturnsAllowed_WhenEntitlementIsGranted()
    {
        var request = new EntitlementCheckRequest("cust-001", "Account.Transfer", "acct-123");

        var response = await fixture.Client.PostAsJsonAsync("/entitlements/check", request);
        var result = await response.Content.ReadFromJsonAsync<EntitlementCheckResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Allowed.Should().BeTrue();
        result.Reason.Should().Be("Permission 'Account.Transfer' granted via role 'AccountOwner'.");
        result.GrantedPermission.Should().Be("Account.Transfer");
    }

    [Fact]
    public async Task Check_ReturnsDenied_WhenPermissionIsIncorrect()
    {
        var request = new EntitlementCheckRequest("cust-001", "Account.Close", "acct-123");

        var response = await fixture.Client.PostAsJsonAsync("/entitlements/check", request);
        var result = await response.Content.ReadFromJsonAsync<EntitlementCheckResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Allowed.Should().BeFalse();
        result.Reason.Should().Be("No matching entitlement found.");
        result.GrantedPermission.Should().BeNull();
    }

    [Fact]
    public async Task Check_ReturnsDenied_WhenResourceIsIncorrect()
    {
        var request = new EntitlementCheckRequest("cust-001", "Account.Transfer", "acct-456");

        var response = await fixture.Client.PostAsJsonAsync("/entitlements/check", request);
        var result = await response.Content.ReadFromJsonAsync<EntitlementCheckResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Allowed.Should().BeFalse();
        result.Reason.Should().Be("No matching entitlement found.");
        result.GrantedPermission.Should().BeNull();
    }

    [Fact]
    public async Task Check_ReturnsDenied_WhenSubjectIsUnknown()
    {
        var request = new EntitlementCheckRequest("cust-999", "Account.View", "acct-123");

        var response = await fixture.Client.PostAsJsonAsync("/entitlements/check", request);
        var result = await response.Content.ReadFromJsonAsync<EntitlementCheckResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Allowed.Should().BeFalse();
        result.Reason.Should().Be("No matching entitlement found.");
        result.GrantedPermission.Should().BeNull();
    }

}


public sealed class EntitlementApiFixture
    : IAsyncLifetime
{
    private readonly Neo4jContainer _container = new Neo4jBuilder("neo4j:5").Build();
    private WebApplicationFactory<Program>? _factory;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        _factory = new EntitlementApiFactory(_container);
        Client = _factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();

        if (_factory is not null) await _factory.DisposeAsync();

        await _container.DisposeAsync();
    }
}


public sealed class EntitlementApiFactory(Neo4jContainer container)
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = container.GetConnectionString(),
                ["Neo4j:Username"] = "neo4j",
                ["Neo4j:Password"] = "password"
            };

            configuration.AddInMemoryCollection(settings);
        });
    }
}
