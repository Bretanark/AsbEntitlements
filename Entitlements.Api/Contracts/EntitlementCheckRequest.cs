namespace Entitlements.Api.Contracts;

public sealed record EntitlementCheckRequest(
    string SubjectId,
    string PermissionName,
    string ResourceId);
