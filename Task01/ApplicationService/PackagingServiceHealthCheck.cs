using Microsoft.Extensions.Diagnostics.HealthChecks;
using Subscription;

namespace PackagingService;

public class PackagingServiceHealthCheck(PackagingGrpcService.PackagingGrpcServiceClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.CheckAccessLimitAsync(new CheckAccessRequest { UserId = "test_user_id", Feature = "health" }, cancellationToken: cancellationToken);

            if (response.Allowed)
                return HealthCheckResult.Healthy("Packaging Service is reachable");
            else
                return HealthCheckResult.Degraded("Packaging Service responded but access denied");

        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Packaging Service unreachable", ex);
        }
    }
}