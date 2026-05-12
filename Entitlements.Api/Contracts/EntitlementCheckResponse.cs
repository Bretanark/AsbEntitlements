namespace Entitlements.Api.Contracts;

public sealed record EntitlementCheckResponse
{
    public required bool IsAllowed { get; init; }
    public string? Reason { get; init; }
    public IReadOnlyList<string> MatchedEntitlements { get; init; } = [];
}
