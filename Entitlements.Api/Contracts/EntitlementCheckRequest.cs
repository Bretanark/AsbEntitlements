namespace Entitlements.Api.Contracts;

public sealed record EntitlementCheckRequest
{
    public required string SubjectId { get; init; }
    public required string Action { get; init; }
    public required string Resource { get; init; }
}
