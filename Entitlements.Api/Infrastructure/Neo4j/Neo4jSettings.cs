namespace Entitlements.Api.Infrastructure.Neo4j;

public sealed class Neo4jSettings
{
    public required string Uri { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public string? Database { get; init; }
}
