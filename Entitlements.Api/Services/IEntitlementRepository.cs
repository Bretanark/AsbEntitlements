using Entitlements.Api.Domain;

namespace Entitlements.Api.Services;

public interface IEntitlementRepository
{
    Task<EntitlementCheckResult?> CheckEntitlementAsync
        (string subjectId, string permissionName, string resourceId, CancellationToken cancellationToken = default);
}
