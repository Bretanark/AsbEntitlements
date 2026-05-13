namespace Entitlements.Api.Contracts;

public sealed record EntitlementCheckResponse(
    bool Allowed,
    string Reason,
    string? GrantedPermission
);
