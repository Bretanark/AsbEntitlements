using Entitlements.Api.Infrastructure.Neo4j;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Entitlements.Api.Seed;

public sealed class DemoDataSeeder(IDriver driver, IOptions<Neo4jSettings> options)
{
    private readonly Neo4jSettings _settings = options.Value;

    public async Task SeedAsync()
    {
        const string clearCypher = """
            MATCH (node)
            WHERE node.demoSeed = true
            DETACH DELETE node
            """;

        const string seedCypher = """
            CREATE
                (cust001:Party {partyId: 'cust-001', demoSeed: true}),
                (accountOwner:PartyRole {name: 'AccountOwner', demoSeed: true}),
                (accountOperations:Entitlement {name: 'AccountOperations', demoSeed: true}),
                (accountTransfer:Permission {name: 'Account.Transfer', demoSeed: true}),
                (accountView123:Permission {name: 'Account.View', demoSeed: true}),
                (acct123:Resource {resourceId: 'acct-123', demoSeed: true}),

                (cust002:Party {partyId: 'cust-002', demoSeed: true}),
                (accountViewer:PartyRole {name: 'AccountViewer', demoSeed: true}),
                (readOnlyAccess:Entitlement {name: 'ReadOnlyAccess', demoSeed: true}),
                (accountView456:Permission {name: 'Account.View', demoSeed: true}),
                (acct456:Resource {resourceId: 'acct-456', demoSeed: true}),

                (cust001)-[:HOLDS_ROLE]->(accountOwner),
                (accountOwner)-[:HAS_ENTITLEMENT]->(accountOperations),
                (accountOperations)-[:GRANTS_PERMISSION]->(accountTransfer),
                (accountOperations)-[:GRANTS_PERMISSION]->(accountView123),
                (accountTransfer)-[:APPLIES_TO]->(acct123),
                (accountView123)-[:APPLIES_TO]->(acct123),

                (cust002)-[:HOLDS_ROLE]->(accountViewer),
                (accountViewer)-[:HAS_ENTITLEMENT]->(readOnlyAccess),
                (readOnlyAccess)-[:GRANTS_PERMISSION]->(accountView456),
                (accountView456)-[:APPLIES_TO]->(acct456)
            """;

        await using var session = _settings.Database is { Length: > 0 }
            ? driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write).WithDatabase(_settings.Database))
            : driver.AsyncSession(o => o.WithDefaultAccessMode(AccessMode.Write));

        await session.ExecuteWriteAsync(
            async tx =>
            {
                var clearCursor = await tx.RunAsync(clearCypher);
                await clearCursor.ConsumeAsync();

                var seedCursor = await tx.RunAsync(seedCypher);
                await seedCursor.ConsumeAsync();
            });
    }
}
