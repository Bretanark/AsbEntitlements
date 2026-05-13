namespace Entitlements.Api.Domain;

public sealed record EntitlementCheckResult(
    bool Allowed,
    string Reason,
    string? GrantedPermission);
