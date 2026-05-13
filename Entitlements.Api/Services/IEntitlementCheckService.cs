using Entitlements.Api.Contracts;

namespace Entitlements.Api.Services;

public interface IEntitlementCheckService
{
    Task<EntitlementCheckResponse> CheckAsync(
        EntitlementCheckRequest request,
        CancellationToken cancellationToken = default);
}
