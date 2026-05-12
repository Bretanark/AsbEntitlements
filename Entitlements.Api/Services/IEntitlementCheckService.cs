using Entitlements.Api.Contracts;
using Entitlements.Api.Domain;

namespace Entitlements.Api.Services;

public interface IEntitlementCheckService
{
    Task<EntitlementCheckResult> CheckAsync(EntitlementCheckRequest request, CancellationToken cancellationToken = default);
}
