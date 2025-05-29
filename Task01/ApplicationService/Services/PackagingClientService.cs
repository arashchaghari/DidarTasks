using Grpc.Core;
using Polly;
using Subscription;

namespace ApplicationService.Services;

public class PackagingClient(PackagingGrpcService.PackagingGrpcServiceClient client)
{
    private readonly AsyncPolicy _resiliencyPolicy = Policy.Handle<RpcException>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))) // Retry policy with exponential backoff
            .WrapAsync(Policy.Handle<RpcException>().CircuitBreakerAsync(2, TimeSpan.FromSeconds(30))); // Circuit breaker policy

    public async Task<CheckAccessResponse> CheckAccessAsync(string userId, string feature)
    {
        return await _resiliencyPolicy.ExecuteAsync(async () =>
            await client.CheckAccessLimitAsync(new CheckAccessRequest { UserId = userId, Feature = feature }));
    }

    public async Task<UpdateSubscriptionResponse> UpdateSubscriptionAsync(string userId, string level)
    {
        return await _resiliencyPolicy.ExecuteAsync(async () =>
            await client.UpdateSubscriptionAsync(new UpdateSubscriptionRequest { UserId = userId, SubscriptionLevel = level }));
    }

    public async Task<GetSubscriptionLevelResponse> GetSubscriptionAsync(string userId)
    {
        return await _resiliencyPolicy.ExecuteAsync(async () =>
            await client.GetSubscriptionLevelAsync(new GetSubscriptionLevelRequest { UserId = userId }));
    }
}