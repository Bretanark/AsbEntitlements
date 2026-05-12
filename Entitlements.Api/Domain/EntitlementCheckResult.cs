using System.Diagnostics.CodeAnalysis;

namespace Entitlements.Api.Domain;

public sealed record EntitlementCheckResult
{
    public EntitlementCheckResult() { }

    [SetsRequiredMembers]
    public EntitlementCheckResult(bool isAllowed, string? reason, IReadOnlyList<string> matchedEntitlements)
    {
        IsAllowed = isAllowed;
        Reason = reason;
        MatchedEntitlements = matchedEntitlements;
    }

    public required bool IsAllowed { get; init; }
    public string? Reason { get; init; }
    public IReadOnlyList<string> MatchedEntitlements { get; init; } = [];
}
