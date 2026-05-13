using Entitlements.Api.Domain;
using Entitlements.Api.Services;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Entitlements.Api.Infrastructure.Neo4j;

public sealed class EntitlementRepository(IDriver driver, IOptions<Neo4jSettings> options)
    : IEntitlementRepository
{
    private readonly Neo4jSettings _settings = options.Value;

    public async Task<EntitlementCheckResult?> CheckEntitlementAsync
        (string subjectId, string permissionName, string resourceId, CancellationToken cancellationToken = default)
    {
        const string cypher = """
            MATCH (party:Party {partyId: $subjectId})
                -[:HOLDS_ROLE]->(role:PartyRole)
                -[:HAS_ENTITLEMENT]->(entitlement:Entitlement)
                -[:GRANTS_PERMISSION]->(permission:Permission {name: $permissionName})
                -[:APPLIES_TO]->(resource:Resource {resourceId: $resourceId})
            RETURN
                role.name AS roleName
            LIMIT 1
            """;

        var parameters = new
        {
            subjectId,
            permissionName,
            resourceId
        };

        await using var session = _settings.Database is { Length: > 0 }
            ? driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read).WithDatabase(_settings.Database))
            : driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));

        var record = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync(cypher, parameters);

                var result = await cursor.FetchAsync();
                if (!result) return null;

                return cursor.Current;
            });

        if (record is null) return null;

        var roleName = record["roleName"].As<string>();
        var reason = $"Permission '{permissionName}' granted via role '{roleName}'.";

        return new EntitlementCheckResult(true, reason, permissionName);
    }

}
