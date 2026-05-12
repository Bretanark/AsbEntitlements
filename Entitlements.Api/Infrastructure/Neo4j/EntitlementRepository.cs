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
            MATCH (party:Party {id: $subjectId})
                --> (role:PartyRole)
                --> (entitlement:Entitlement)
                --> (permission:Permission {name: $permissionName})
                --> (resource:Resource {id: $resourceId})
            RETURN
                coalesce(entitlement.id, entitlement.name) AS entitlementId,
                entitlement.name AS entitlementName
            LIMIT 1
            """;

        var parameters = new { subjectId, permissionName, resourceId };

        await using var session = _settings.Database is { Length: > 0 }
            ? driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read).WithDatabase(_settings.Database))
            : driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Read));

        var record = await session.ExecuteReadAsync(
            async tx =>
            {
                var cursor = await tx.RunAsync(cypher, parameters);

                var result = await cursor.FetchAsync();
                if (!result) return null;

                return cursor.Current;
            });

        if (record is null) return null;

        var entitlementId = record["entitlementId"].As<string?>();
        var entitlementName = record["entitlementName"].As<string?>();
        var matchedEntitlement = entitlementName ?? entitlementId ?? "Entitlement";

        return new EntitlementCheckResult(true, "Matching entitlement found.", [matchedEntitlement]);
    }

}
