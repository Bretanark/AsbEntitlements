using Entitlements.Api.Contracts;

namespace Entitlements.Api.Services;

public sealed class EntitlementCheckService(IEntitlementRepository repository)
    : IEntitlementCheckService
{
    public async Task<EntitlementCheckResponse> CheckAsync(EntitlementCheckRequest request, CancellationToken cancellationToken = default)
    {
        var result = await repository.CheckEntitlementAsync(request.SubjectId, request.PermissionName, request.ResourceId, cancellationToken);

        if (result is null)
            return new EntitlementCheckResponse(false, "No matching entitlement found.", null);

        return new EntitlementCheckResponse(result.Allowed, result.Reason, result.GrantedPermission);
    }
}
